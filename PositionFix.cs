using UnityEngine;

namespace REPOLevelScaler
{
    internal class PositionFix : MonoBehaviour
    {
        internal GameObject ObjectRef = null!;
        internal LevelPoint levelPoint = null!;
        internal Vector3 originalPos = Vector3.zero;
        internal Vector3 originalScale = Vector3.zero;

        internal void UpdateObject(ref GameObject gameObject)
        {
            ObjectRef = gameObject;

            if (originalPos == Vector3.zero)
                originalPos = gameObject.transform.position;

            if (originalScale == Vector3.zero)
                originalScale = gameObject.transform.localScale;

            gameObject.transform.localScale = originalScale * LevelScaler.instance.Scaler;
        }

        internal void UpdatePoint(ref LevelPoint point, GameObject gameObject)
        {
            levelPoint = point;
            ObjectRef = gameObject;
            if (originalPos == Vector3.zero)
                originalPos = levelPoint.transform.position;

            levelPoint.transform.parent = gameObject.transform;
            levelPoint.transform.position = originalPos / LevelScaler.instance.Scaler;
            //levelPoint.transform.localScale = LevelScaler.instance.ChosenScale;
            //levelPoint.transform.position = new(original.position.x * LevelScaler.instance.Scaler, original.position.y * LevelScaler.instance.Scaler, original.position.z * LevelScaler.instance.Scaler);
        }

        internal void UpdateEnemy(ref GameObject enemyObject)
        {
            ObjectRef = enemyObject;

            if (originalPos == Vector3.zero)
                originalPos = enemyObject.transform.position;

            if (originalScale == Vector3.zero)
                originalScale = enemyObject.transform.localScale;

            enemyObject.transform.localScale = originalScale * LevelScaler.instance.Scaler;
            //enemyObject.transform.position = originalPos / LevelScaler.instance.Scaler;
        }

        /*
        internal void ClampToObject(Transform transform, Transform parent)
        {
            Vector3 localPos = parent.InverseTransformPoint(original.position);

            // Calculate min/max allowed positions (considering child size)
            Vector3 minAllowed = -parent.localScale / 2 + original.localScale / 2;
            Vector3 maxAllowed = parent.localScale / 2 - original.localScale / 2;

            // Clamp the local position
            localPos.x = Mathf.Clamp(localPos.x, minAllowed.x, maxAllowed.x);
            localPos.y = Mathf.Clamp(localPos.y, minAllowed.y, maxAllowed.y);
            localPos.z = Mathf.Clamp(localPos.z, minAllowed.z, maxAllowed.z);

            // Convert back to world space
            transform.position = parent.TransformPoint(localPos);
        }*/
    }
}
