

//-------------------------------------------------------------------------------
// Fill animation combo box
int CDisplay::FillAnimList(void)
{
	if (0 != g_pcAsset->pcScene->mNumAnimations)
	{
		// now fill in all animation names
		for (unsigned int i = 0; i < g_pcAsset->pcScene->mNumAnimations;++i)	{
			SendDlgItemMessage(g_hDlg,IDC_COMBO1,CB_ADDSTRING,0,
				( LPARAM ) g_pcAsset->pcScene->mAnimations[i]->mName.data);
		}

		// also add a dummy - 'none'
		SendDlgItemMessage(g_hDlg,IDC_COMBO1,CB_ADDSTRING,0,(LPARAM)"none");

		// select first
		SendDlgItemMessage(g_hDlg,IDC_COMBO1,CB_SETCURSEL,0,0);

		EnableAnimTools(TRUE);
	}
	else // tools remain disabled
		EnableAnimTools(FALSE);

	return 1;
}

//-------------------------------------------------------------------------------
// Clear the list of animations
int CDisplay::ClearAnimList(void)
{
	// clear the combo box
	SendDlgItemMessage(g_hDlg,IDC_COMBO1,CB_RESETCONTENT,0,0);
	return 1;
}
//-------------------------------------------------------------------------------
// Clear the tree view
int CDisplay::ClearDisplayList(void)
{
	// clear the combo box
	TreeView_DeleteAllItems(GetDlgItem(g_hDlg,IDC_TREE1));
	this->Reset();
	return 1;
}

