using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;


public class MaxCountOdinDrawer : OdinAttributeDrawer<MaxCountAttribute>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (Property.ChildResolver is IOrderedCollectionResolver listResolver)
        {
            var currentLength = listResolver.MaxCollectionLength;
            var maxLenght = Attribute.MaxCount;

            if (currentLength > maxLenght)
            {
                listResolver.QueueRemoveAt(currentLength - 1);
                listResolver.ApplyChanges();
            }
            
            var newLabel = $"{label.text} ({currentLength}/{maxLenght})";
            
            if (currentLength >= maxLenght)
            {
                Sirenix.Utilities.Editor.SirenixEditorGUI.InfoMessageBox(
                    $"Maximum capacity reached");
            }
            
            CallNextDrawer(new GUIContent(newLabel));
        }
        else
        {
            CallNextDrawer(label);
        }
    }
}