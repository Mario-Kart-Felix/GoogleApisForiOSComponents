// Firebase artifacts available to be built.
Artifact FIREBASE_AB_TESTING_ARTIFACT;
Artifact FIREBASE_AD_MOB_ARTIFACT;
Artifact FIREBASE_ANALYTICS_ARTIFACT;
Artifact FIREBASE_AUTH_ARTIFACT;
Artifact FIREBASE_CLOUD_FIRESTORE_ARTIFACT;
Artifact FIREBASE_CLOUD_MESSAGING_ARTIFACT;
Artifact FIREBASE_CORE_ARTIFACT;
Artifact FIREBASE_CRASHLYTICS_ARTIFACT;
Artifact FIREBASE_DATABASE_ARTIFACT;
Artifact FIREBASE_DYNAMIC_LINKS_ARTIFACT;
Artifact FIREBASE_INSTANCE_ID_ARTIFACT;
Artifact FIREBASE_INVITES_ARTIFACT;
Artifact FIREBASE_MLKIT_ARTIFACT;
Artifact FIREBASE_MLKIT_COMMON_ARTIFACT;
Artifact FIREBASE_MLKIT_MODEL_INTERPRETER_ARTIFACT;
Artifact FIREBASE_PERFORMANCE_MONITORING_ARTIFACT;
Artifact FIREBASE_REMOTE_CONFIG_ARTIFACT;
Artifact FIREBASE_STORAGE_ARTIFACT;

// Google artifacts available to be built.
Artifact GOOGLE_ANALYTICS_ARTIFACT;
Artifact GOOGLE_APP_INDEXING_ARTIFACT;
Artifact GOOGLE_CAST_ARTIFACT;
Artifact GOOGLE_CORE_ARTIFACT;
Artifact GOOGLE_INSTANCE_ID_ARTIFACT;
Artifact GOOGLE_MAPS_ARTIFACT;
Artifact GOOGLE_MOBILE_ADS_ARTIFACT;
Artifact GOOGLE_PLACES_ARTIFACT;
Artifact GOOGLE_SIGN_IN_ARTIFACT;
Artifact GOOGLE_TAG_MANAGER_ARTIFACT;

var ARTIFACTS = new Dictionary<string, Artifact> ();

void DeserializeArtifacts ()
{
	try
	{
		var artifacts = new List<Artifact> ();
		var xmlRoot = new XmlRootAttribute ("Artifacts");
		var serializer = new XmlSerializer (typeof (List<Artifact>), xmlRoot);

		// A FileStream is needed to read the XML document.
		using (FileStream fileStream = new FileStream ("./components.xml", FileMode.Open)) {
			var reader = XmlReader.Create (fileStream);
			artifacts = (List<Artifact>)serializer.Deserialize (reader);
		}

		foreach (var artifact in artifacts)
			ARTIFACTS.Add (artifact.Id, artifact);
	}
	catch (Exception e)
	{
		throw e;
	}
}

void SerializeArtifacts ()
{
	try {
		var xmlRoot = new XmlRootAttribute ("Artifacts");
		var artifacts = new List<Artifact> (ARTIFACTS.Values);
		var serializer = new XmlSerializer (typeof (List<Artifact>), xmlRoot);

		using (var fileStream = new FileStream ("./components.xml", FileMode.Create)) {
			TextWriter writer = new StreamWriter (fileStream);
			serializer.Serialize(writer, artifacts);
			writer.Close();
		}
	} catch (Exception e) {
		Console.WriteLine (e);
	}
}

void SetArtifacts ()
{
	foreach (var artifact in ARTIFACTS.Values)
	{
		switch (artifact.Id)
		{
		case "Firebase.ABTesting": FIREBASE_AB_TESTING_ARTIFACT = artifact; break;
		case "Firebase.AdMob": FIREBASE_AD_MOB_ARTIFACT = artifact; break;
		case "Firebase.Analytics": FIREBASE_ANALYTICS_ARTIFACT = artifact; break;
		case "Firebase.Auth": FIREBASE_AUTH_ARTIFACT = artifact; break;
		case "Firebase.CloudFirestore": FIREBASE_CLOUD_FIRESTORE_ARTIFACT = artifact; break;
		case "Firebase.CloudMessaging": FIREBASE_CLOUD_MESSAGING_ARTIFACT = artifact; break;
		case "Firebase.Core": FIREBASE_CORE_ARTIFACT = artifact; break;
		case "Firebase.Crashlytics": FIREBASE_CRASHLYTICS_ARTIFACT = artifact; break;
		case "Firebase.Database": FIREBASE_DATABASE_ARTIFACT = artifact; break;
		case "Firebase.DynamicLinks": FIREBASE_DYNAMIC_LINKS_ARTIFACT = artifact; break;
		case "Firebase.InstanceID": FIREBASE_INSTANCE_ID_ARTIFACT = artifact; break;
		case "Firebase.Invites": FIREBASE_INVITES_ARTIFACT = artifact; break;
		case "Firebase.MLKit": FIREBASE_MLKIT_ARTIFACT = artifact; break;
		case "Firebase.MLKit.Common": FIREBASE_MLKIT_COMMON_ARTIFACT = artifact; break;
		case "Firebase.MLKit.ModelInterpreter": FIREBASE_MLKIT_MODEL_INTERPRETER_ARTIFACT = artifact; break;
		case "Firebase.PerformanceMonitoring": FIREBASE_PERFORMANCE_MONITORING_ARTIFACT = artifact; break;
		case "Firebase.RemoteConfig": FIREBASE_REMOTE_CONFIG_ARTIFACT = artifact; break;
		case "Firebase.Storage": FIREBASE_STORAGE_ARTIFACT = artifact; break;
		
		case "Google.Analytics": GOOGLE_ANALYTICS_ARTIFACT = artifact; break;
		case "Google.AppIndexing": GOOGLE_APP_INDEXING_ARTIFACT = artifact; break;
		case "Google.Cast": GOOGLE_CAST_ARTIFACT = artifact; break;
		case "Google.Core": GOOGLE_CORE_ARTIFACT = artifact; break;
		case "Google.InstanceID": GOOGLE_INSTANCE_ID_ARTIFACT = artifact; break;
		case "Google.Maps": GOOGLE_MAPS_ARTIFACT = artifact; break;
		case "Google.MobileAds": GOOGLE_MOBILE_ADS_ARTIFACT = artifact; break;
		case "Google.Places": GOOGLE_PLACES_ARTIFACT = artifact; break;
		case "Google.SignIn": GOOGLE_SIGN_IN_ARTIFACT = artifact; break;
		case "Google.TagManager": GOOGLE_TAG_MANAGER_ARTIFACT = artifact; break;
		}
	}
}

void SetArtifactsDependencies ()
{
	FIREBASE_AB_TESTING_ARTIFACT.Dependencies              = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT };
	FIREBASE_AD_MOB_ARTIFACT.Dependencies                  = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT, GOOGLE_MOBILE_ADS_ARTIFACT };
	FIREBASE_ANALYTICS_ARTIFACT.Dependencies               = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT };
	FIREBASE_AUTH_ARTIFACT.Dependencies                    = new [] { FIREBASE_CORE_ARTIFACT };
	FIREBASE_CLOUD_FIRESTORE_ARTIFACT.Dependencies         = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT };
	FIREBASE_CLOUD_MESSAGING_ARTIFACT.Dependencies         = new [] { FIREBASE_CORE_ARTIFACT };
	FIREBASE_CORE_ARTIFACT.Dependencies                    = null;
	FIREBASE_CRASHLYTICS_ARTIFACT.Dependencies             = null;
	FIREBASE_DATABASE_ARTIFACT.Dependencies                = new [] { FIREBASE_CORE_ARTIFACT };
	FIREBASE_DYNAMIC_LINKS_ARTIFACT.Dependencies           = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT };
	FIREBASE_INSTANCE_ID_ARTIFACT.Dependencies             = new [] { FIREBASE_CORE_ARTIFACT };
	FIREBASE_INVITES_ARTIFACT.Dependencies                 = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT, FIREBASE_DYNAMIC_LINKS_ARTIFACT, GOOGLE_SIGN_IN_ARTIFACT };
	FIREBASE_MLKIT_ARTIFACT.Dependencies                   = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_MLKIT_COMMON_ARTIFACT };
	FIREBASE_MLKIT_COMMON_ARTIFACT.Dependencies            = new [] { FIREBASE_CORE_ARTIFACT };
	FIREBASE_MLKIT_MODEL_INTERPRETER_ARTIFACT.Dependencies = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_MLKIT_COMMON_ARTIFACT };
	FIREBASE_PERFORMANCE_MONITORING_ARTIFACT.Dependencies  = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT };
	FIREBASE_REMOTE_CONFIG_ARTIFACT.Dependencies           = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT, FIREBASE_AB_TESTING_ARTIFACT };
	FIREBASE_STORAGE_ARTIFACT.Dependencies                 = new [] { FIREBASE_CORE_ARTIFACT };

	GOOGLE_ANALYTICS_ARTIFACT.Dependencies    = null;
	GOOGLE_APP_INDEXING_ARTIFACT.Dependencies = null;
	GOOGLE_CAST_ARTIFACT.Dependencies         = new [] { FIREBASE_CORE_ARTIFACT };
	GOOGLE_CORE_ARTIFACT.Dependencies         = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT };
	GOOGLE_INSTANCE_ID_ARTIFACT.Dependencies  = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT, GOOGLE_CORE_ARTIFACT };
	GOOGLE_MAPS_ARTIFACT.Dependencies         = null;
	GOOGLE_MOBILE_ADS_ARTIFACT.Dependencies   = null;
	GOOGLE_PLACES_ARTIFACT.Dependencies       = new [] { GOOGLE_MAPS_ARTIFACT };
	GOOGLE_SIGN_IN_ARTIFACT.Dependencies      = new [] { FIREBASE_CORE_ARTIFACT };
	GOOGLE_TAG_MANAGER_ARTIFACT.Dependencies  = new [] { FIREBASE_CORE_ARTIFACT, FIREBASE_INSTANCE_ID_ARTIFACT, FIREBASE_ANALYTICS_ARTIFACT, GOOGLE_ANALYTICS_ARTIFACT };
}
