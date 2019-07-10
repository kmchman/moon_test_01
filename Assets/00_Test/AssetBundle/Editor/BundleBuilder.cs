using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BundleBuilder : Editor
{
    [MenuItem("Assets/ Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(Constant.TestAssetRoot, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);

        //BuildPipeline.BuildAssetBundles(@"C:\Users\David\Dev\AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        //BuildPipeline.BuildAssetBundles("Assets /AssetBundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

    }

}
