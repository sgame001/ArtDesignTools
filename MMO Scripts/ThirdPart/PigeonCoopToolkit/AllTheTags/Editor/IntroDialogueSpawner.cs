using PigeonCoopToolkit.Generic.Editor;
using UnityEditor;

namespace PigeonCoopToolkit.AllTheTags.Internal.Editor
{ 
    [InitializeOnLoad]
    public class IntroDialogueSpawner : InfoDialogueSpawner
    {
        private static IntroDialogueSpawner _instance;
        static IntroDialogueSpawner() 
        {
            _instance = new IntroDialogueSpawner();
            _instance.SetParams(
                "All The Tags!",
                "PCTK/AllTheTags/banner",
                new Generic.VersionInformation("All The Tags!", 1, 2, 1),
                "/PigeonCoopToolkit/__AllTheTags Extra/Pigeon Coop Toolkit - AllTheTags!.pdf",
                "25715");
        }

        [MenuItem("Window/Pigeon Coop Toolkit/All The Tags!/About")]
        public static void About()
        {
            _instance.LaunchAbout();
        }
    }
}

