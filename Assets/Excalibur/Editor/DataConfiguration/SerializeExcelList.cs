using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using NPOI.POIFS.Properties;

namespace Excalibur
{
	public class SerializeExcelList : TreeView
	{
        TreeViewState state;
        private bool contextOnItem = false;
        DataConfigurationEditorWin parent;

        public SerializeExcelList(TreeViewState state, DataConfigurationEditorWin parent) : base(state)
        {
            this.state = state;
            this.parent = parent;
            parent.data_SO.onReloadSerializeTree -= Reload;
            parent.data_SO.onReloadSerializeTree += Reload;
            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            //if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            //{
            //    SetSelection(new int[0], TreeViewSelectionOptions.FireSelectionChanged);
            //}
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return item != null && item.displayName.Length > 0;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);

            if (args.newName.Length > 0 && args.newName != args.originalName)
            {
                args.acceptedRename = true;
                if (!parent.data_SO.OnRenameEndToCheck(args.newName))
                {
                    args.acceptedRename = false;
                    EditorUtility.DisplayDialog("Rename Error", "重名了，请重新命名！", "Confirm");
                }
            }
            else
            {
                args.acceptedRename = false;
                EditorUtility.DisplayDialog("Rename Error", "名字不合规", "Confirm");
            }

            if (args.acceptedRename)
            {
                parent.data_SO.OnRenameSerilizeData(args.originalName, args.newName);
                Reload();
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem { id = -1, depth = -1 };
            if (parent.data_SO.SerializeExcels.Count == 0)
            {
                root.AddChild(new TreeViewItem { id = 0, depth = -1 });
            }
            else
            {
                for (int i = 0; i < parent.data_SO.SerializeExcels.Count; ++i)
                {
                    ExcelTreeItem newItem = new ExcelTreeItem(i, parent.data_SO.SerializeExcels[i]);
                    root.AddChild(newItem);
                }
            }
            return root;
            //SetupDepthsFromParentsAndChildren(root);
        }

        protected override void ContextClicked()
        {
            if (contextOnItem)
            {
                contextOnItem = false;
                return;
            }

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Import"), false, CreateNewDataToSerialize);
            menu.ShowAsContext();
        }

        protected override void ContextClickedItem(int id)
        {
            contextOnItem = true;

            TreeViewItem item = FindItem(id, rootItem);
            if (item == null)
                return;
            GenericMenu menu = new GenericMenu();
            if (item is ExcelTreeItem)
            {
                menu.AddItem(new GUIContent("Delete"), false, DeletedData, item);
            }
            menu.AddItem(new GUIContent("Add Import"), false, CreateNewDataToSerialize);
            menu.ShowAsContext();
        }

        private void CreateNewDataToSerialize()
        {
            string newItemName = $"newData{parent.data_SO.ValidEndName}";
            parent.data_SO.AddSerializeExcel(newItemName);
            Reload();
            //Log.General("Create Data To Serialize.");
        }

        private void DeletedData(object context)
        {
            IList<int> selection = GetSelection();
            if (selection.Count > 1)
            {
                parent.data_SO.RemoveSerializeExcel(selection);
                Reload();
            }
            else
            {
                ExcelTreeItem item = context as ExcelTreeItem;
                if (parent.data_SO.RemoveSerializeExcel(item.ExcelName))
                {
                    Reload();
                    return;
                }
                EditorUtility.DisplayDialog("Error", $"导入数据中不存在{item.ExcelName}的表", "Confirm");
            }
        }
    }
}
