using System.Collections;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MMI
{
    public class CreateObjectAction : IGameAction
    {
        Vector3 _position;
        Vector3 _scale;
        Color _color;
        PrimitiveType _shape;
        GameObject _createdObject;
        Material _material;

        public CreateObjectAction(Vector3 pos, Vector3 scale, Material material, string colorName, string shapeName)
        {
            _position = pos;
            _scale = scale;
            _material = material;
            _color = StringToColor(colorName);
            _shape = StringToPrimitiveType(shapeName);
        }

        private Color StringToColor(string colorName)
        {
            Color color;
            if (!ColorUtility.TryParseHtmlString(colorName.ToLower(), out color))
            {
                Debug.LogError("Invalid color name " + colorName);
                return Color.grey;
            }
            return color;
        }

        private PrimitiveType StringToPrimitiveType(string shapeName)
        {
            switch (shapeName)
            {
                case "cube":
                    return PrimitiveType.Cube;
                case "sphere":
                    return PrimitiveType.Sphere;
                case "capsule":
                    return PrimitiveType.Capsule;
                case "cylinder":
                    return PrimitiveType.Cylinder;
                default:
                    Debug.LogError("Invalid shape name " + shapeName);
                    return PrimitiveType.Plane;
            }
        }
        public void Execute()
        {
            if (_shape == PrimitiveType.Plane)
                return;
            _createdObject = GameObject.CreatePrimitive(_shape);
            _createdObject.transform.position = _position;
            _createdObject.transform.localScale = _scale;
            var renderer = _createdObject.GetComponent<Renderer>();
            renderer.material = _material;
            var interactable = _createdObject.AddComponent<InteractableObject>();
            interactable.UpdateColor(_color);
        }
    }
}