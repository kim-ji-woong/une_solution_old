import React, { Component } from 'react';
import ColText from '../columns/colText';
import ColComboBox from '../columns/colComboBox';
import ColCheckBox from '../columns/colCheckBox';

import styles from '../../../Common/css/style.module.css';
import teamEditors from '../../css/teamEditor.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';
import TeamEditorResource from '../../resource/id';

class ColTemporaryMemberNew extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            member: null,
            //jobPositions: null,
            //roles: null,
        };

        this.props = props;
    }

    componentDidUpdate(prevProps, prevState) {

    }

    onChangeCheckBox = (checked) => {
        let member = this.props.member;
        member.check = checked;

        //this.setState({ member: member });
        //this.props.onChange(this.props.index, member);
        return;
    }

    onChangeRole = (role) => {
        this.props.member.role = role;
    }

    onChangeDisplaySOPName = (displaySOPName) => {
        this.props.member.displaySOPName = displaySOPName;
    }

    openPopup = (columnName) => {
        const member = this.props.member;
        this.props.onChangeMemberEditMode(this.props.member, columnName, true);
        this.props.openPopup(member);
    }

    onChangeMemberEditMode = (columnName) => {
        let isEditMode = true;
        if (this.props.member.editType === columnName) {
            isEditMode = false;
        }

        this.props.onChangeMemberEditMode(this.props.member, columnName, isEditMode);
    }

    render() {        
        let regularTeamName = "";
        let regularMemberName = "";
        let jobPositionName = "";

        const member = this.props.member;
        //let btnEdit1 = null;
        //if (member.editType === TeamEditorResource.ID.colTextMode.regularTeamName && member.isEditMode) {
        //    btnEdit1 = <a onClick={this.openPopup}>편집</a>
        //}

        //let btnEdit2 = null;
        //if (member.editType === TeamEditorResource.ID.colTextMode.regularMemberName && member.isEditMode) {
        //    btnEdit2 = <a onClick={this.openPopup}>편집</a>
        //}

        if (member.regular !== null && member.regular !== undefined)
            regularTeamName = member.regular.teamName;

        if (member.regularMember !== null && member.regularMember !== undefined) {
            const regularMember = member.regularMember;
            regularMemberName = regularMember.memberName;

            const jobPositions = this.props.jobPositions;

            for (let i = 0; i < jobPositions.length; i++) {
                let jobPosition = jobPositions[i];

                if (regularMember.jobPositionID === jobPosition.value) {
                    jobPositionName = jobPosition.name;
                    break;
                }
            }
        }
            
        return (
            <>
                <ColCheckBox
                    defaultChecked={member.check}
                    isEditMode={member.isEditMode}
                    onChange={this.onChangeCheckBox}
                />
                <td><span>{this.props.index + 1}</span></td>
                <ColComboBox
                    value={member.role} options={this.props.roles} member={member}
                    columnName={TeamEditorResource.ID.colTextMode.role} isEditMode={member.isEditMode} editColumnName={member.editType}
                    onChangeMemberEditMode={this.props.onChangeMemberEditMode} onChangeMember={this.props.onChangeMember}
                />
                <ColText
                    value={member.displaySOPName} member={member}
                    columnName={TeamEditorResource.ID.colTextMode.displaySOPName} isEditMode={member.isEditMode} editColumnName={member.editType}
                    onChangeMemberEditMode={this.props.onChangeMemberEditMode} onChangeMember={this.props.onChangeMember}
                />
                <td>
                    <span className={styles.fixation + " " + teamEditors.colTextLink} onMouseDown={() => this.openPopup(TeamEditorResource.ID.colTextMode.regularTeamName)}>{regularTeamName}</span>
                    {
                        //<div className={uneStyles.sctEdtt}>
                        //    <p className={uneStyles.editBtn}>
                        //        {regularTeamName}
                        //    </p>
                        //    {btnEdit1}
                        //</div>
                    }
                </td>
                <td>
                    <span>{jobPositionName}</span>
                </td>
                <td>
                    <span className={styles.fixation + " " + teamEditors.colTextLink} onMouseDown={() => this.openPopup(TeamEditorResource.ID.colTextMode.regularMemberName)}>{regularMemberName}</span>
                    {
                        //<div className={uneStyles.sctEdtt}>
                        //    <p className={uneStyles.editBtn} onMouseDown={() => this.onChangeMemberEditMode(member, TeamEditorResource.ID.colTextMode.regularMemberName)}>{regularMemberName}</p>
                        //    {btnEdit2}
                        //</div>
                    }
                </td>
            </>
        );
    }
}

export default ColTemporaryMemberNew;