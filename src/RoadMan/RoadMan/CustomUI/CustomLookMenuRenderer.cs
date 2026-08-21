using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace UnE.Utility
{
    internal class MenuColor : ProfessionalColorTable
    {
        private Color mMenuSelectedColor = Color.FromArgb(154, 149, 168);

   
        public override Color MenuItemSelected
        {
            get { return mMenuSelectedColor; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return mBackColor; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.White; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return mBackColor; }
        }

        private Color mBackColor = Color.FromArgb(75, 71, 86);
        public override Color ButtonPressedGradientBegin
        {
            get { return mBackColor; }
        }

        public override Color MenuBorder
        {
            get { return mBackColor; }
        }
       
        public override Color MenuItemBorder 
        {
            get { return mBackColor; }
        }

		public override Color ToolStripPanelGradientEnd
		{
			get { return mBackColor; }
		}
		public override Color ToolStripPanelGradientBegin
		{
			get { return Color.White; }
		}

		public override Color ToolStripGradientBegin
		{
			get { return mBackColor; }
		}

		public override Color ToolStripGradientMiddle
		{
			get { return mBackColor; }
		}

		public override Color ToolStripGradientEnd 
		{
			get { return mBackColor; }
		}

		public override Color ToolStripBorder
		{
			get { return mBackColor; }
		}
		public override Color GripDark
		{
			get { return mBackColor; }
		}
		//
		// 요약:
		//     그립(이동 핸들)에 대한 강조 효과에 사용할 색을 가져옵니다.
		//
		// 반환 값:
		//     그립(이동 핸들)에 대한 강조 효과에 사용할 색을 나타내는 System.Drawing.Color입니다.
		public override Color GripLight
		{
			get { return mBackColor; }
		}

		/*
		public virtual Color ButtonCheckedGradientBegin { get; }
		//
		// 요약:
		//     단추를 선택 표시한 경우 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시한 경우 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonCheckedGradientEnd { get; }
		//
		// 요약:
		//     단추를 선택 표시한 경우 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시한 경우 사용되는 그라데이션의 중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonCheckedGradientMiddle { get; }
		//
		// 요약:
		//     단추를 선택 표시한 경우 사용되는 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시한 경우 사용되는 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonCheckedHighlight { get; }
		//
		// 요약:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonCheckedHighlight에 사용할 테두리
		//     색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonCheckedHighlight에 사용할 테두리
		//     색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonCheckedHighlightBorder { get; }
		//
		// 요약:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientBegin, System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientMiddle
		//     및 System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientEnd 색에
		//     사용할 테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientBegin, System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientMiddle
		//     및 System.Windows.Forms.ProfessionalColorTable.ButtonPressedGradientEnd 색에
		//     사용할 테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedBorder { get; }
		//
		// 요약:
		//     단추를 누른 경우 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 누른 경우 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedGradientBegin { get; }
		//
		// 요약:
		//     단추를 누른 경우 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 누른 경우 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedGradientEnd { get; }
		//
		// 요약:
		//     단추를 누른 경우 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 누른 경우 사용되는 그라데이션의 중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedGradientMiddle { get; }
		//
		// 요약:
		//     단추를 누른 경우 사용되는 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 누른 경우 사용되는 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedHighlight { get; }
		//
		// 요약:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonPressedHighlight에 사용할 테두리
		//     색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonPressedHighlight에 사용할 테두리
		//     색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonPressedHighlightBorder { get; }
		//
		// 요약:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientBegin,
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientMiddle
		//     및 System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientEnd 색에
		//     사용할 테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientBegin,
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientMiddle
		//     및 System.Windows.Forms.ProfessionalColorTable.ButtonSelectedGradientEnd 색에
		//     사용할 테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedBorder { get; }
		//
		// 요약:
		//     단추를 선택한 경우 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택한 경우 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedGradientBegin { get; }
		//
		// 요약:
		//     단추를 선택한 경우 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택한 경우 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedGradientEnd { get; }
		//
		// 요약:
		//     단추를 선택한 경우 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택한 경우 사용되는 그라데이션의 중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedGradientMiddle { get; }
		//
		// 요약:
		//     단추를 선택한 경우 사용되는 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택한 경우 사용되는 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedHighlight { get; }
		//
		// 요약:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedHighlight에 사용할
		//     테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ProfessionalColorTable.ButtonSelectedHighlight에 사용할
		//     테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ButtonSelectedHighlightBorder { get; }
		//
		// 요약:
		//     단추를 선택 표시하고 그라데이션이 사용되는 경우 사용할 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시하고 그라데이션이 사용되는 경우 사용할 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color CheckBackground { get; }
		//
		// 요약:
		//     단추를 선택 표시 및 선택하고 그라데이션이 사용되는 경우 사용할 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시 및 선택하고 그라데이션이 사용되는 경우 사용할 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color CheckPressedBackground { get; }
		//
		// 요약:
		//     단추를 선택 표시 및 선택하고 그라데이션이 사용되는 경우 사용할 단색을 가져옵니다.
		//
		// 반환 값:
		//     단추를 선택 표시 및 선택하고 그라데이션이 사용되는 경우 사용할 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color CheckSelectedBackground { get; }
		//
		// 요약:
		//     그립(이동 핸들)에 대한 그림자 효과에 사용할 색을 가져옵니다.
		//
		// 반환 값:
		//     그립(이동 핸들)에 대한 그림자 효과에 사용할 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color GripDark { get; }
		//
		// 요약:
		//     그립(이동 핸들)에 대한 강조 효과에 사용할 색을 가져옵니다.
		//
		// 반환 값:
		//     그립(이동 핸들)에 대한 강조 효과에 사용할 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color GripLight { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 시작 색을 나타내는
		//     System.Drawing.Color입니다.
		public virtual Color ImageMarginGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 끝 색을 나타내는
		//     System.Drawing.Color입니다.
		public virtual Color ImageMarginGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의 중간 색을 나타내는
		//     System.Drawing.Color입니다.
		public virtual Color ImageMarginGradientMiddle { get; }
		//
		// 요약:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     시작 색을 가져옵니다.
		//
		// 반환 값:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ImageMarginRevealedGradientBegin { get; }
		//
		// 요약:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     끝 색을 가져옵니다.
		//
		// 반환 값:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ImageMarginRevealedGradientEnd { get; }
		//
		// 요약:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     중간 색을 가져옵니다.
		//
		// 반환 값:
		//     항목이 표시될 때 System.Windows.Forms.ToolStripDropDownMenu의 이미지 여백에 사용되는 그라데이션의
		//     중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ImageMarginRevealedGradientMiddle { get; }
		//
		// 요약:
		//     System.Windows.Forms.MenuStrip에 사용할 테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.MenuStrip에 사용할 테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuBorder
		{
			get {  get { return mBackColor; }
		}
		//
		// 요약:
		//     System.Windows.Forms.ToolStripMenuItem에 사용할 테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripMenuItem에 사용할 테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuItemBorder 
		{
			get { return mBackColor; }
		}
		//
		// 요약:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 시작 색을 나타내는
		//     System.Drawing.Color입니다.
		public virtual Color MenuItemPressedGradientBegin { get; }
		//
		// 요약:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuItemPressedGradientEnd { get; }
		//
		// 요약:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     최상위 System.Windows.Forms.ToolStripMenuItem을 누른 경우 사용되는 그라데이션의 중간 색을 나타내는
		//     System.Drawing.Color입니다.
		public virtual Color MenuItemPressedGradientMiddle { get; }
		//
		// 요약:
		//     최상위 System.Windows.Forms.ToolStripMenuItem 이외의 다른 System.Windows.Forms.ToolStripMenuItem을
		//     선택한 경우 사용할 단색을 가져옵니다.
		//
		// 반환 값:
		//     최상위 System.Windows.Forms.ToolStripMenuItem 이외의 다른 System.Windows.Forms.ToolStripMenuItem을
		//     선택한 경우 사용할 단색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuItemSelected 
		{ 
			get { return base.MenuItem;
		}
		//
		// 요약:
		//     System.Windows.Forms.ToolStripMenuItem을 선택한 경우 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripMenuItem을 선택한 경우 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuItemSelectedGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripMenuItem을 선택한 경우 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripMenuItem을 선택한 경우 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuItemSelectedGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.MenuStrip에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.MenuStrip에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuStripGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.MenuStrip에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.MenuStrip에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color MenuStripGradientEnd { get; }
		 
		//
		// 요약:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color OverflowButtonGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color OverflowButtonGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripOverflowButton에 사용되는 그라데이션의 중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color OverflowButtonGradientMiddle { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripContainer에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripContainer에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color RaftingContainerGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripContainer에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripContainer에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color
		//     입니다.
		public virtual Color RaftingContainerGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripSeparator에 대한 그림자 효과에 사용할 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripSeparator에 대한 그림자 효과에 사용할 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color SeparatorDark { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripSeparator의 강조 효과에 사용되는 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripSeparator에 대한 강조 효과에 사용할 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color SeparatorLight { get; }
		//
		// 요약:
		//     System.Windows.Forms.StatusStrip에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.StatusStrip에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color StatusStripGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.StatusStrip에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.StatusStrip에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color StatusStripGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStrip의 아래쪽 가장자리에 사용할 테두리 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStrip의 아래쪽 가장자리에 사용할 테두리 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripBorder { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripContentPanel에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripContentPanel에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripContentPanelGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripContentPanel에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripContentPanel에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripContentPanelGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripDropDown의 단색 배경을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripDropDown의 단색 배경을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripDropDownBackground { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripGradientEnd { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 중간 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStrip 배경에 사용되는 그라데이션의 중간 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripGradientMiddle { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripPanel에 사용되는 그라데이션의 시작 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripPanel에 사용되는 그라데이션의 시작 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripPanelGradientBegin { get; }
		//
		// 요약:
		//     System.Windows.Forms.ToolStripPanel에 사용되는 그라데이션의 끝 색을 가져옵니다.
		//
		// 반환 값:
		//     System.Windows.Forms.ToolStripPanel에 사용되는 그라데이션의 끝 색을 나타내는 System.Drawing.Color입니다.
		public virtual Color ToolStripPanelGradientEnd { get; }
 
		*/
	}


    public class CustomLookMenuRenderer : ToolStripProfessionalRenderer 
    {
        private Color mBackColor = Color.FromArgb(52, 73, 94);
        public Color BackColor
        {
            get { return mBackColor; }
            set { mBackColor = value; }
        }

        private Color mLineColor = Color.White;
        public Color LineColor
        {
            get { return mLineColor; }
            set { mLineColor = value; }
        }

        private int mLineGap = 15;        
        public int LineGap
        {
            get { return mLineGap; }
            set { mLineGap = value; }
        }

        private float mLineThick = 1.0f;
        public float LineThick
        {
            get { return mLineThick; }
            set { mLineThick = value; }
        }

        private static MenuColor mMenuColor = new MenuColor();
        public CustomLookMenuRenderer() : base(mMenuColor)
        {            
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        { 
            if (e.Vertical || (e.Item as ToolStripSeparator) == null) 
            {
                base.OnRenderSeparator(e);   
            }
            else
            {
                SolidBrush blueBrush = new SolidBrush(mBackColor);
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(blueBrush, bounds);
                blueBrush.Dispose();

                Pen pen = new Pen(mLineColor, mLineThick);

                int x = bounds.X + mLineGap;
                int endx = bounds.Width - mLineGap;
                if (endx < 1)
                    endx = 1;
                int y = (bounds.Bottom - bounds.Top) / 2;
                e.Graphics.DrawLine(pen, x, y, endx, y);                
                pen.Dispose(); 
              
            }
        } 
    }
}
