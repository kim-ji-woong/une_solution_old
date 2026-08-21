import React, { Component } from 'react';
import styles from '../../Common/css/style.module.css';
import bodyStyles from '../../Common/css/style.module.css';
import SopManagerContent from './sopManagerContent';

import $ from 'jquery';
import SopManagerBody from './sopManagerBody';
import Footer from './footer';
import SopManagerResource from '../resource/id';
import OpenSOPOptions from './popup/openSOPOptions';
import SaveSOPOptions from './popup/saveSOPOptions';
import SopController from '../services/sopController';

import SessionString from '../../Common/js/sessionString';
import SopDataManager from '../services/sopDataManager';
import { TeamEditController } from '../../TeamEditor/services/teamEditController';

class SopManager extends Component {
    static cssStyles = styles;
    static menu = {
        none: null,
        editSOP: SopManagerResource.ID.menu.editSOP,
        newSOP: SopManagerResource.ID.menu.newSOP,
        open: SopManagerResource.ID.menu.open,
        save: SopManagerResource.ID.menu.save,
        saveAs: SopManagerResource.ID.menu.saveAs,
        delete: SopManagerResource.ID.menu.delete,
        openXML: SopManagerResource.ID.menu.openXML,
        saveXML: SopManagerResource.ID.menu.saveXML
    }

    constructor(props)
    {
        super(props);

        this.state = {
            content: props.menu,
            menuDatas: null,
            showCascading:
            {
                actionStep: false,
                addComponent: false,
                specialCharacter: false,
                userDefined: false
            },
            loginUser: null,
            sopData: null,
            prevProps: props
        }

        this.props = props;
        this.refFileDialog = React.createRef();
    }

    componentDidMount()
    {
        $('html, body').css({ 'display': 'block', 'height': '100%', 'overflow': 'hidden', 'color': '#000', 'font-size': '14px' });
        $('#subPage').css({ background:'#fff'});

        // 각 페이지 별로 클래스 초기화
        $('#subPage').addClass('sop');

        const user = { id: 1 };
        //const user = JSON.parse(window.localStorage.getItem(SessionString.Key.account));
        this.setState({loginUser: user});
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        return {
            content: props.menu,
            menuDatas: state.menuDatas,
            showCascading:
            {
                actionStep: state.showCascading.actionStep,
                addComponent: state.showCascading.addComponent,
                specialCharacter: state.showCascading.specialCharacter,
                userDefined: state.showCascading.userDefined
            },
            loginUser: state.loginUser,
            sopData: state.sopData,
            prevProps: props
        };
    }

    changeContent = (content, menuDatas, showDlg) => {
        if (content === SopManager.menu.editSOP) {
            if (menuDatas) {
                this.setState(
                    {
                        content: content,
                        sopData: menuDatas,
                        menuDatas: menuDatas,
                        showCascading:
                        {
                            actionStep: true,
                            addComponent: this.state.showCascading.addComponent,
                            specialCharacter: this.state.showCascading.specialCharacter,
                            userDefined: this.state.showCascading.userDefined
                        }
                    }
                );
            }
            else {
                this.setState({ content: content, sopData: menuDatas, menuDatas: menuDatas });
            }
        }
        else if (content === SopManager.menu.save) {
            if (showDlg) {
                this.setState({ content: content, sopData: menuDatas, menuDatas: menuDatas });
            }
            else {
                this.saveDB(menuDatas);
            }
        }
        else if (content === SopManager.menu.saveXML) {
            this.saveXML(menuDatas);
        }
        else if (content === SopManager.menu.open) {
            if (menuDatas === null) {
                this.setState({ content: content, menuDatas: menuDatas });
            }
            else {
                this.openDB(menuDatas);
            }
        }
        else if (content === SopManager.menu.openXML) {
            this.openXML();
        }
        else {
            this.setState({ content: content, menuDatas: menuDatas });
        }
    }

    async openDB(versionID) {
        const [sopDataResult, message] = await SopController.requestOpenDB(versionID);

        if (sopDataResult && sopDataResult.success) {
            // 새로 읽어들인 SopData를 새로운 Grid에 그리기 위하여 이전 Grid는 삭제한다.
            this.clearSOP();

            // 수신자 정보를 알아내기 위하여 팀 정보를 미리 얻어온다.
            sopDataResult.sopData.teamAllTreeDatas = await this.getAllTreeDatas();
            SopDataManager.setReceiverNames(sopDataResult.sopData);

            this.setCurrentActionStep(sopDataResult.sopData);
            this.checkArrows(sopDataResult.sopData);
            await this.checkStepMembers(sopDataResult.sopData);
            this.setState(
                {
                    content: SopManager.menu.editSOP,
                    sopData: sopDataResult.sopData,
                    menuDatas: sopDataResult.sopData,
                    showCascading:
                    {
                        actionStep: true,
                        addComponent: this.state.showCascading.addComponent,
                        specialCharacter: this.state.showCascading.specialCharacter,
                        userDefined: this.state.showCascading.userDefined
                    }
                });
        }
        else {
            this.setState({ content: SopManager.menu.editSOP, menuDatas: this.state.sopData });
            alert(message);
        }
    }

    async getAllTreeDatas() {
        const teamAllTreeDatas = {};
        
        teamAllTreeDatas.regular = await TeamEditController.DisplayRegular();
        teamAllTreeDatas.normal = await TeamEditController.DisplayTemporary(true);
        teamAllTreeDatas.emergency = await TeamEditController.DisplayTemporary(false);

        return teamAllTreeDatas;
    }

    openXML() {
        this.refFileDialog.current.click();
    }

    async checkStepMembers(sopData) {
        if (sopData) {
            const actionStepCount = sopData.actionStepDatas.length;
            
            for (let i = 0; i < actionStepCount; i++) {
                const actionStepData = sopData.actionStepDatas[i];

                if (actionStepData.stepMemberDatas.length === 0) {
                    const [stepMemberData, message] = await SopController.requestDefaultStepMemberData(actionStepData);

                    if (!stepMemberData) {
                        alert(message);
                        break;
                    }
                }
            }
        }
    }

    checkArrows(sopData) {
        if (sopData) {
            const actionStepCount = sopData.actionStepDatas.length;

            for (let i = 0; i < actionStepCount; i++) {
                const actionStepData = sopData.actionStepDatas[i];
                const stepMemberCount = actionStepData.stepMemberDatas.length;

                for (let j = 0; j < stepMemberCount; j++) {
                    const stepMemberData = actionStepData.stepMemberDatas[j];

                    if (stepMemberData.arrows.length > 0) {
                        stepMemberData.resetArrows = true;
                    }
                }
            }
        }
    }

    setCurrentActionStep(sopData) {
        sopData.actionStepDatas.map(actionStepData => {
            if (actionStepData.actionStep) {
                sopData.currentActionStep = actionStepData;
            }
        });
    }

    changeCascadingMode = (cascading, show) => {
        if (cascading === SopManagerResource.ID.cascadingMenu.actionStep) {
            this.setState({
                showCascading:
                {
                    actionStep: show,
                    addComponent: this.state.showCascading.addComponent,
                    specialCharacter: this.state.showCascading.specialCharacter,
                    userDefined: this.state.showCascading.userDefined
                }
            });
        }
        else if (cascading === SopManagerResource.ID.cascadingMenu.addComponent) {
            this.setState({
                showCascading:
                {
                    actionStep: this.state.showCascading.actionStep,
                    addComponent: show,
                    specialCharacter: this.state.showCascading.specialCharacter,
                    userDefined: this.state.showCascading.userDefined
                }
            });
        }
        else if (cascading === SopManagerResource.ID.cascadingMenu.specialCharacter) {
            this.setState({
                showCascading:
                {
                    actionStep: this.state.showCascading.actionStep,
                    addComponent: this.state.showCascading.addComponent,
                    specialCharacter: show,
                    userDefined: this.state.showCascading.userDefined
                }
            });
        }
        else if (cascading === SopManagerResource.ID.cascadingMenu.userDefined) {
            this.setState({
                showCascading:
                {
                    actionStep: this.state.showCascading.actionStep,
                    addComponent: this.state.showCascading.addComponent,
                    specialCharacter: this.state.showCascading.specialCharacter,
                    userDefined: show
                }
            });
        }
    }

    onSelectFile = (event) => {
        const file = event.target.files[0];
        this.refFileDialog.current.value = "";
        this._openXML(file);
    }

    async _openXML(file) {
        if (file) {
            const [sopDataResult, message] = await SopController.requestOpenXML(file);

            if (sopDataResult && sopDataResult.success) {
                // 새로 읽어들인 SopData를 새로운 Grid에 그리기 위하여 이전 Grid는 삭제한다.
                this.clearSOP();

                // 수신자 정보를 알아내기 위하여 팀 정보를 미리 얻어온다.
                sopDataResult.sopData.teamAllTreeDatas = await this.getAllTreeDatas();
                SopDataManager.setReceiverNames(sopDataResult.sopData);

                this.setCurrentActionStep(sopDataResult.sopData);
                this.checkArrows(sopDataResult.sopData);
                await this.checkStepMembers(sopDataResult.sopData);
                this.setState(
                    {
                        content: SopManager.menu.editSOP,
                        sopData: sopDataResult.sopData,
                        menuDatas: sopDataResult.sopData,
                        showCascading:
                        {
                            actionStep: true,
                            addComponent: this.state.showCascading.addComponent,
                            specialCharacter: this.state.showCascading.specialCharacter,
                            userDefined: this.state.showCascading.userDefined
                        }
                    });
            }
            else {
                this.setState({ content: SopManager.menu.editSOP, menuDatas: this.state.sopData });
                alert(message);
            }
        }
    }

    clearSOP() {
        this.setState(
            {
                content: SopManager.menu.editSOP,
                sopData: null,
                menuDatas: null,
                showCascading:
                {
                    actionStep: true,
                    addComponent: this.state.showCascading.addComponent,
                    specialCharacter: this.state.showCascading.specialCharacter,
                    userDefined: this.state.showCascading.userDefined
                }
            });
    }

    async saveXML(sopData) {
        if (!sopData) {
            return;
        }

        const [sopDataResult, message] = await SopController.requestSaveXML(sopData);

        if (sopDataResult === null) {
            alert(message);
        }
    }

    async saveDB(sopData) {
        if (!sopData) {
            return;
        }

        const userID = this.state.loginUser ? this.state.loginUser.id : -1;
        const [sopDataResult, message] = await SopController.requestSaveDB(userID, sopData);

        if (sopDataResult && sopDataResult.success) {
            this.checkArrows(sopDataResult.sopData);
            await this.checkStepMembers(sopDataResult.sopData);

            // 수신자 정보를 알아내기 위하여 팀 정보를 미리 얻어온다.
            sopDataResult.sopData.teamAllTreeDatas = await this.getAllTreeDatas();
            SopDataManager.setReceiverNames(sopDataResult.sopData);

            this.setState(
                {
                    content: SopManager.menu.editSOP,
                    menuDatas: sopDataResult.sopData,
                    showCascading:
                    {
                        actionStep: true,
                        addComponent: this.state.showCascading.addComponent,
                        specialCharacter: this.state.showCascading.specialCharacter,
                        userDefined: this.state.showCascading.userDefined
                    },
                    sopData: sopDataResult.sopData
                }
            );
        }
        else {
            this.setState({ content: SopManager.menu.editSOP, menuDatas: sopData });
            alert(message);
        }
    }

    getPopup() {
        if (this.state.content === SopManager.menu.open) {
            return <OpenSOPOptions sopData={this.state.sopData} content={this.changeContent} />;
        }
        else if (this.state.content === SopManager.menu.open) {
            return <SaveSOPOptions sopData={this.state.sopData} content={this.changeContent} />;
        }

        return <></>;
    }

    render() {
        return (
            <div id="subPage">
                <input ref={this.refFileDialog} className={bodyStyles.hidden} type='file' accept='.sop' onChange={this.onSelectFile} />
                <div id={styles.subAside} className={styles.sop}>
                    <SopManagerContent sopData={this.state.sopData} content={this.changeContent} loginUser={this.state.loginUser} />
                </div>
                <SopManagerBody menu={this.state.content} menuDatas={this.state.menuDatas} sopData={this.state.sopData} showCascading={this.state.showCascading} changeCascadingMode={this.changeCascadingMode} content={this.changeContent} loginUser={this.state.loginUser} />
                <Footer />
                {
                    this.getPopup()
                }
            </div>

        );
    }
}

export default SopManager;