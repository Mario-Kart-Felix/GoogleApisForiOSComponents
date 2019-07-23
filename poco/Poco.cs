using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Poco
{
	public enum ComponentGroup
	{
		[XmlEnum]
		Firebase,
		[XmlEnum]
		Google
	}

	public enum FrameworkSource
	{
		[XmlEnum]
		Targets,
		[XmlEnum]
		Pods,
		[XmlEnum]
		Custom
	}

	public struct Platform
	{
		#region Properties

		public string Arch { get; private set; }
		public string Sdk { get; private set; }

		public static Platform iOSSimulator { get; } = new Platform("i386", "iphonesimulator");
		public static Platform iOSSimulator64 { get; } = new Platform("x86_64", "iphonesimulator");
		public static Platform iOSArmV7 { get; } = new Platform("armv7", "iphoneos");
		public static Platform iOSArmV7s { get; } = new Platform("armv7s", "iphoneos");
		public static Platform iOSArm64 { get; } = new Platform("arm64", "iphoneos");

		#endregion

		#region Constructors

		Platform(string arch, string sdk)
		{
			Arch = arch;
			Sdk = sdk;
		}

		#endregion

		#region Public Functionality

		public static Platform Create (string arch, string sdk) => new Platform(arch, sdk);
		public override string ToString () => $"{Arch} => {Sdk}";

		#endregion

		#region Operations

		public static bool operator == (Platform first, Platform second) =>
			string.Equals (first.ToString (), second.ToString ());

		public static bool operator != (Platform first, Platform second) =>
			!string.Equals (first.ToString (), second.ToString ());

		public override bool Equals(object obj) =>
			obj is Platform platform && this == platform;

		public override int GetHashCode () =>
			base.GetHashCode ();

		#endregion
	}

	public class PodSpec
	{
		// The podspec name
		[XmlAttribute]
		public string Name { get; set; }
		// The podspec version if any, the component version otherwise.
		[XmlAttribute]
		public string Version { get; set; }
		// Target used to build the Xcode Pods project.
		// If null, Name property value will be used.
		[XmlAttribute]
		public string TargetName { get; set; }
		// Overrides the default framework's name built with Pods project and Xcode.
		// If null, Name property value will be used.
		[XmlAttribute]
		public string FrameworkName { get; set; }
		// The desired subspec to be used.
		// If null, default subspecs defined within the podspec will be used.
		[XmlArray]
		[XmlArrayItem ("SubSpec")]
		public string [] SubSpecs { get; set; }
		// If true and when Subspecs property is not null, default subspecs
		// defined within the podspec will added to the Podfile. Otherwise,
		// only subSpecs specified in Subspecs will be used. False by default.
		[XmlAttribute]
		public bool UseDefaultSubSpecs { get; set; }
		// Specify the source where the framework will be gotten.
		// From a .targets file by default.
		[XmlAttribute]
		public FrameworkSource FrameworkSource { get; set; }
		// If true, the podspec can be built using Xcode. True by default.
		[XmlAttribute]
		public bool CanBeBuild { get; set; }

		public PodSpec () { }

		public PodSpec (string name, string version, string targetName = null, string frameworkName = null, FrameworkSource frameworkSource = FrameworkSource.Targets, string [] subSpecs = null, bool useDefaultSubSpecs = false, bool canBeBuild = true)
		{
			Name = name;
			Version = version;
			TargetName = targetName ?? name;
			FrameworkName = frameworkName ?? name;
			FrameworkSource = frameworkSource;
			SubSpecs = subSpecs;
			UseDefaultSubSpecs = useDefaultSubSpecs;
			CanBeBuild = canBeBuild;
		}
	}

	public class Artifact : IEquatable<Artifact>
	{
		// The id of the component.
		[XmlAttribute]
		public string Id { get; set; }
		// The version to be published on NuGet.
		[XmlAttribute]
		public string NugetVersion { get; set; }
		// The minimun iOS supported version of the component.
		[XmlAttribute]
		public string MinimunSupportedVersion { get; set; }
		// If it's a Firebase or Google component.
		[XmlAttribute]
		public ComponentGroup ComponentGroup { get; set; }
		// The C# project name. This will have the Id property value if not specified.
		[XmlAttribute]
		public string CsprojName { get; set; }
		// Other Google/Firebase components that make this component work.
		[XmlIgnoreAttribute]
		public Artifact [] Dependencies { get; set; }
		// The component build order.
		[XmlIgnoreAttribute]
		public int BuildOrder { get => Dependencies?.Length + 1 ?? 1; }
		// The specs used in the Podfile.
		[XmlArray]
		[XmlArrayItem ("PodSpec")]
		public PodSpec [] PodSpecs { get; set; }

		public Artifact () { }

		public Artifact (string id, string nugetVersion, string minimunSupportedVersion, ComponentGroup componentType, string csprojName = null, Artifact [] dependencies = null, PodSpec [] podSpecs = null)
		{
			Id = id;
			NugetVersion = nugetVersion;
			MinimunSupportedVersion = minimunSupportedVersion;
			ComponentGroup = componentType;
			CsprojName = csprojName ?? id;
			Dependencies = dependencies;
			PodSpecs = podSpecs;
		}

		public bool Equals(Artifact other)
		{
			if (Object.ReferenceEquals(other, null)) return false;
			if (Object.ReferenceEquals(this, other)) return true;

			return Id == other.Id;
		}

		public override int GetHashCode ()
		{
			int hashCode = Id.GetHashCode ();
			return hashCode ^ hashCode;
		}
	}

	public class Artifacts
	{
		[XmlArray ("Artifacts")]
		[XmlArrayItem ("Artifact")]
		public List<Artifact> ArtifactsList { get; set; }
	}

}
