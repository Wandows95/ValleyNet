namespace ValleyNet.Core.Server.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;


    //[CustomEditor(typeof(ServerSettings))]
    public class ServerSettingsInspector : Editor
    {
        public override void OnInspectorGUI() 
        {
            //ServerSettings settings = (ServerSettings)target;
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fixedHeight = 100f;
            guiStyle.fixedWidth = 100f;
            
            // Gamemode Section
            //EditorGUILayout.LabelField(new GUIContent((Resources.Load(settings.currentGamemode.iconDirectory) as Texture)), guiStyle); // Gamemode Icon
            // Space for Image
            /*for(int i = 0; i < 15; i++)
            {
                EditorGUILayout.Space();
            }*/
    //EditorGUILayout.LabelField(settings.currentGamemode.name, EditorStyles.boldLabel);
            //settings.currentGamemode.maxPlayers = EditorGUILayout.IntField("Max Players:", settings.currentGamemode.maxPlayers);
            //EditorGUILayout.Space();
            // Server Settings Section
            //EditorGUILayout.LabelField("Server Settings", EditorStyles.boldLabel);
            //settings.serverPort = EditorGUILayout.IntField("Server Port:", settings.serverPort);
            //settings.MOTD = EditorGUILayout.TextField("Message of the Day (MOTD)", settings.MOTD);
            //settings.tickrate = EditorGUILayout.IntField("Tickrate (Per Second): ", settings.tickrate);

            //settings.runInBackground = EditorGUILayout.Toggle(new GUIContent("Run In Background", "Run server when tabbed out? (Recommended: Enabled)"), settings.runInBackground);
        }
    }
}