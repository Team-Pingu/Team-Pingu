using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Builder
{
    [MenuItem("/Build/Windows/Server")]
    static public void buildWindowsServer() {
        string[] scenes = {"Assets/Level/Scenes/Netcode/Server.unity", "Assets/Level/Scenes/MainScene.unity"};
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions{
            scenes = scenes,
            locationPathName = "Build/Windows/Server/TeamPingu.exe",
            target = BuildTarget.StandaloneWindows64,
            subtarget = (int) StandaloneBuildSubtarget.Player,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }


    [MenuItem("Build/Windows/Client")]
    static public void buildWindowsClient() {
        string[] scenes = {"Assets/Level/Scenes/Netcode/Client.unity", "Assets/Level/Scenes/MainScene.unity"};
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions{
            scenes = scenes,
            locationPathName = "Build/Windows/Client/TeamPingu.exe",
            target = BuildTarget.StandaloneWindows64,
            subtarget = (int) StandaloneBuildSubtarget.Player,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("/Build/Linux/Server")]
    static public void buildLinuxServer() {
        string[] scenes = {"Assets/Level/Scenes/Netcode/Server.unity", "Assets/Level/Scenes/MainScene.unity"};
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions{
            scenes = scenes,
            locationPathName = "Build/Linux/Server/TeamPingu.exe",
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int) StandaloneBuildSubtarget.Player,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }


    [MenuItem("Build/Linux/Client")]
    static public void buildLinuxClient() {
        string[] scenes = {"Assets/Level/Scenes/Netcode/Client.unity", "Assets/Level/Scenes/MainScene.unity"};
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions{
            scenes = scenes,
            locationPathName = "Build/Linux/Client/TeamPingu.exe",
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int) StandaloneBuildSubtarget.Player,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
