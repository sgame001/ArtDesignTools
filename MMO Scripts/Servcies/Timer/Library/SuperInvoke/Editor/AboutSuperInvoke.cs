using UnityEditor;

public class AboutSuperInvoke : EditorWindow {

    [MenuItem("Help/About SuperInvoke")]
    public static void ShowWindow() {
        EditorUtility.DisplayDialog("About SuperInvoke",
                                    "SuperInvoke Version 2.0 \n" +
                                     "Copyright \u00a9 2016 Jacob Games. \n " +
                                     "All rights reserved.",
                                    "OK");
    }
}
