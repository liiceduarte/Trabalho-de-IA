using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPointGrid))]
public class WayPointGridEditor : Editor
{
    // Valores de entreda para criar o grid
    static float WayPointDistance = 5.0f;
    static int GridSize = 32;

    // Propriedades internas da classe (WayPointGrid) que serão atualizar no inspetor customizavel
    SerializedProperty EditorStyle;
    SerializedProperty LayerMaskCollision;
    SerializedProperty LayerMaskUpdateHeight;
    SerializedProperty WayPointHeightOffSet;
    SerializedProperty WayPointPrefab;

    public override void OnInspectorGUI()
    {
        // Desenhando linhas
        Rect rect = EditorGUILayout.GetControlRect(false, 5.0f);
        rect.height = 5.0f;

        EditorGUI.DrawRect(rect, new Color(1.0f, 0.5f, 0.5f, 1.0f));

        // Adicionando um espaço

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Desenhando configurações de criação do grid
        WayPointDistance = EditorGUILayout.FloatField("Distance between waypoint", WayPointDistance);
        GridSize = EditorGUILayout.IntSlider("Size of the grid", GridSize, 0, 256);

        if (GUILayout.Button("Create Grid"))
            CreateGrid();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        rect.y += 87.5f;
        EditorGUI.DrawRect(rect, new Color(1.0f, 0.5f, 0.5f, 1.0f));
    
        if (GUILayout.Button("Load Grid"))
            LoadGrid();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        rect.y += 35.0f;
        EditorGUI.DrawRect(rect, new Color(1.0f, 0.5f, 0.5f, 1.0f));

        if (GUILayout.Button("Update Grid Collision and Height"))
            UpdateGrid();

        EditorGUILayout.Space();

        // Pegandos as propriedas serializables
        LayerMaskCollision = serializedObject.FindProperty("LayerMaskCollision");
        LayerMaskUpdateHeight = serializedObject.FindProperty("LayerMaskUpdateHeight");
        EditorStyle = serializedObject.FindProperty("EditorStyle");
        WayPointHeightOffSet = serializedObject.FindProperty("WayPointHeightOffSet");
        WayPointPrefab = serializedObject.FindProperty("WayPointPrefab");

        // Criando os campos para alterar as propriedades do objeto atual
        serializedObject.Update();
        EditorGUILayout.PropertyField(LayerMaskCollision);
        EditorGUILayout.PropertyField(LayerMaskUpdateHeight);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        rect.y += 85.0f;
        EditorGUI.DrawRect(rect, new Color(1.0f, 0.5f, 0.5f, 1.0f));

        EditorGUILayout.PropertyField(EditorStyle);
        EditorGUILayout.PropertyField(WayPointHeightOffSet);
        EditorGUILayout.PropertyField(WayPointPrefab);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        rect.y += 75.0f;
        EditorGUI.DrawRect(rect, new Color(1.0f, 0.5f, 0.5f, 1.0f));
    }

    void CreateGrid()
    {
        WayPointGrid waypoint = (WayPointGrid) target;
        waypoint.CreateWaypointGrid(GridSize, WayPointDistance);
    }

    void UpdateGrid()
    {
        WayPointGrid waypoint = (WayPointGrid) target;
        waypoint.UpdateWayPointGridCollision();
    }

    void LoadGrid()
    {
        WayPointGrid waypoint = (WayPointGrid) target;
        waypoint.LoadGrid();
    }
}
