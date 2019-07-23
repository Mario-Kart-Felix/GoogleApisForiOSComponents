#tool nuget:?package=XamarinComponent&version=1.1.0.65

#addin nuget:?package=Cake.XCode&version=4.0.0
#addin nuget:?package=Cake.Xamarin.Build&version=4.0.1
#addin nuget:?package=Cake.Xamarin&version=3.0.0
#addin nuget:?package=Cake.FileHelpers&version=3.0.0

#r "./Poco.dll"

using Poco;
using System.Xml;
using System.Xml.Serialization;

#load "common.cake"
#load "components.cake"
#load "custom_externals_download.cake"

var TARGET = Argument ("t", Argument ("target", "build"));
var SDKS = Argument ("sdks", "");

var SOLUTION_PATH = "./Xamarin.Google.sln";

// Artifacts that need to be built from pods or be copied from pods
var ARTIFACTS_TO_BUILD = new List<Artifact> ();

var SOURCES_TARGETS = new List<string> ();
var SAMPLES_TARGETS = new List<string> {
	@"Firebase\\AdMobSample",
	@"Firebase\\AnalyticsSample",
	@"Firebase\\AuthSample",
	@"Firebase\\CloudFirestoreSample",
	@"Firebase\\CloudMessagingSample",
	@"Firebase\\CrashlyticsSample",
	@"Firebase\\DatabaseSample",
	@"Firebase\\DynamicLinksSample",
	@"Firebase\\InvitesSample",
	@"Firebase\\ModelInterpreterSample",
	@"Firebase\\MLKitSample",
	@"Firebase\\PerformanceMonitoringSample",
	@"Firebase\\RemoteConfigSample",
	@"Firebase\\StorageSample",
	@"Google\\CuteAnimalsiOS",
	@"Google\\AppIndexingSample",
	@"Google\\CastSample",
	@"Google\\InstanceIDSample",
	@"Google\\GoogleMapsAdvSample",
	@"Google\\GoogleMapsSample",
	@"Google\\MobileAdsExample",
	@"Google\\GooglePlacesSample",
	@"Google\\SignInExample",
	@"Google\\TagManagerSample"
};

// Podfile basic structure
var PODFILE_BEGIN = new [] {
	"platform :ios, '{0}'",
	"install! 'cocoapods', :integrate_targets => false",
	"use_frameworks!",
	"target 'XamarinGoogle' do",
};
var PODFILE_END = new [] {
	"end",
};

FilePath GetCakeToolPath ()
{
	var possibleExe = GetFiles ("./**/tools/Cake/Cake.exe").FirstOrDefault ();
	if (possibleExe != null)
		return possibleExe;
		
	var p = System.Diagnostics.Process.GetCurrentProcess ();	
	return new FilePath (p.Modules[0].FileName);
}

void BuildCake (string target)
{
	var cakeSettings = new CakeSettings { 
		ToolPath = GetCakeToolPath (),
		Arguments = new Dictionary<string, string> { { "target", target }, { "sdks", SDKS } },
		Verbosity = Verbosity.Diagnostic
	};

	// Run the script from the subfolder
	CakeExecuteScript ("./build.cake", cakeSettings);
}

// From Cake.Xamarin.Build, dumps out versions of things
LogSystemInfo ();

Task("build")
	.Does(() =>
{
	BuildCake ("nuget");
	BuildCake ("samples");
});

// Prepares the artifacts to be built.
// From CI will always build everything but, locally you can customize what
// you build, just to save some time when testing locally.
Task("prepare-artifacts")
	.IsDependeeOf("externals")
	.Does(() =>
{
	DeserializeArtifacts ();
	SetArtifacts ();
	SetArtifactsDependencies ();

	var orderedArtifactsForBuild = new List<Artifact> ();

	if (string.IsNullOrWhiteSpace (SDKS) || TARGET == "samples") {
		orderedArtifactsForBuild.AddRange (ARTIFACTS.Values);
	} else {
		var sdks = SDKS.Split (',');
		foreach (var sdk in sdks) {
			if (!(ARTIFACTS.ContainsKey (sdk) && ARTIFACTS [sdk] is Artifact artifact))
				throw new Exception($"The {sdk} component does not exist.");
			
			orderedArtifactsForBuild.Add (artifact);
			AddArtifactDependencies (orderedArtifactsForBuild, artifact.Dependencies);
		}

		orderedArtifactsForBuild = orderedArtifactsForBuild.Distinct ().ToList ();
	}

	orderedArtifactsForBuild.Sort ((f, s) => s.BuildOrder.CompareTo (f.BuildOrder));
	ARTIFACTS_TO_BUILD.AddRange (orderedArtifactsForBuild);

	Information ("Build order:");

	foreach (var artifact in ARTIFACTS_TO_BUILD) {
		SOURCES_TARGETS.Add($@"{artifact.ComponentGroup}\\{artifact.CsprojName.Replace ('.', '_')}");
		Information (artifact.Id);
	}
});

Task ("externals")
	.WithCriteria (!DirectoryExists ("./externals/"))
	.Does (() => 
{
	EnsureDirectoryExists ("./externals/");

	Information ("////////////////////////////////////////");
	Information ("// Pods Repo Update Started           //");
	Information ("////////////////////////////////////////");
	
	Information ("\nUpdating Cocoapods repo...");
	CocoaPodRepoUpdate ();

	Information ("////////////////////////////////////////");
	Information ("// Pods Repo Update Ended             //");
	Information ("////////////////////////////////////////");

	foreach (var artifact in ARTIFACTS_TO_BUILD) {
		UpdateVersionInCsproj (artifact);
		CreateAndInstallPodfile (artifact);
		BuildSdkOnPodfile (artifact);
	}

	// Call here custom methods created at custom_externals_download.cake file
	// to download frameworks and/or bundles for the artifact
});

Task ("libs")
	.IsDependentOn("externals")
	.Does(() =>
{
	CleanVisualStudioSolution ();

	var targets = $@"source\\{string.Join (@";source\\", SOURCES_TARGETS)}";

	MSBuild(SOLUTION_PATH, c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.MaxCpuCount = 0;
		c.Targets.Clear();
		c.Targets.Add(targets);
	});
});

Task ("samples")
	.IsDependentOn("libs")
	.Does(() =>
{
	var targets = $@"samples\\{string.Join (@";samples\\", SAMPLES_TARGETS)}";

	MSBuild(SOLUTION_PATH, c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.MaxCpuCount = 0;
		c.Targets.Clear();
		c.Targets.Add(targets);
	});
});

Task ("nuget")
	.IsDependentOn("libs")
	.Does(() =>
{
	EnsureDirectoryExists("./output");

	var targets = $@"source\\{string.Join (@":Pack;source\\", SOURCES_TARGETS)}:Pack";

	MSBuild(SOLUTION_PATH, c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.MaxCpuCount = 0;
		c.Targets.Clear();
		c.Targets.Add(targets);
		c.Properties.Add("PackageOutputPath", new [] { "../../../output/" });
	});
});

Task ("clean")
	.Does (() => 
{
	CleanVisualStudioSolution ();

	var deleteDirectorySettings = new DeleteDirectorySettings {
		Recursive = true,
		Force = true
	};

	if (DirectoryExists ("./externals/"))
		DeleteDirectory ("./externals", deleteDirectorySettings);

	if (DirectoryExists ("./output/"))
		DeleteDirectory ("./output", deleteDirectorySettings);
});

Teardown (context =>
{
	var artifacts = GetFiles ("./output/**/*");

	if (artifacts?.Count () <= 0)
		return;

	Information ($"Found Artifacts ({artifacts.Count ()})");
	foreach (var a in artifacts)
		Information ("{0}", a);
});

RunTarget (TARGET);
