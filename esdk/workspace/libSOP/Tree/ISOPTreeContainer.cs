using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.SOP.Tree
{
    public interface ISOPTreeContainer
    {
        bool IgnoreLoadSOP
        {
            get;
            set;
        }

        // nActionStepID가 사용되는 Tree인지 확인하여(isNormal, isRegular)
        // 다른 버전일 경우 다시 로딩하도록 한다.
        bool ReloadTree(int nActionStepID, out bool isRegular, out bool isNormal);
        TreeNode FindActionStepNode(int ActionStepID);

        void ChangeTab(int nActionStepID);

        void SelectSop(TreeNode selnode);

        bool LoadSOP(TreeNode nodeDisaster, int nActionStepID);


        void SetSelectPath(string szNodePath);

        void SetScenarioName(int nActionStepID);

        void SelectNode(TreeNode node);

    }
}
