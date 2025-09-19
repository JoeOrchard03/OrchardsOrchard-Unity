using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CursorHotspotPreview : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public float previewScale = 0.5f; // scale for scene view preview

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (cursorTexture == null) return;

        // Draw a red cross for hotspot
        Vector3 pos = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos + Vector3.up * 0.1f, pos - Vector3.up * 0.1f);
        Gizmos.DrawLine(pos + Vector3.right * 0.1f, pos - Vector3.right * 0.1f);
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawCursorPreview(CursorHotspotPreview preview, GizmoType gizmoType)
    {
        if (preview.cursorTexture == null) return;

        // Draw texture in Scene view
        Handles.BeginGUI();

        Vector3 screenPos = HandleUtility.WorldToGUIPoint(preview.transform.position);
        Rect rect = new Rect(
            screenPos.x - preview.hotspot.x * preview.previewScale,
            screenPos.y - preview.hotspot.y * preview.previewScale,
            preview.cursorTexture.width * preview.previewScale,
            preview.cursorTexture.height * preview.previewScale
        );

        GUI.DrawTexture(rect, preview.cursorTexture);

        Handles.EndGUI();
    }
#endif
}