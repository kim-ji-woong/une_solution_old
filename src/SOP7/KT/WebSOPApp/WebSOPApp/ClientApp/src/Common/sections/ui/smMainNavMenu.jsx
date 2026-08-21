import React, { Component } from 'react';
import SMMenuBtn from './smComponentMenuBtn';
import '../css/app.css';
import SopManager from './sopManager';
import SopControler from '../services/sopController';

class SMMainNavMenu extends Component {
    constructor(props) {
        super(props);
        this.state = {
            menuButtons:
                [
                    { isActive: false, dataType: "newSOP", menuName: "새 SOP" },
                    { isActive: false, dataType: "openSOP", menuName: "열기" },
                    { isActive: false, dataType: "saveSOP", menuName: "저장" },
                    { isActive: false, dataType: "deleteSOP", menuName: "삭제" }
                ],
        };
    }

    onClickMenu = (dataType) => {
        const buttons = [...this.state.menuButtons];
        const selectedMenu = buttons.find(menu => menu.dataType === dataType);

        if (selectedMenu) {
            if (selectedMenu.dataType === "newSOP") {
                this.props.onSelectMenu(SopManager.Menu_New_SOP);
                //this.newSOP();
            }
            else if (selectedMenu.dataType === "saveSOP") {
                this.props.onSelectMenu(SopManager.Menu_Save_SOP/*, null*/);
            }
            else if (selectedMenu.dataType === "openSOP") {
                this.props.onSelectMenu(SopManager.Menu_Open_SOP);
            }
        }
    }

    //async newSOP() {
    //    const disasterCategories = await SopControler.newSOP();

    //    if (disasterCategories !== null) {
    //        this.props.onSelectMenu(SopManager.Menu_New_SOP, disasterCategories);
    //    }

    //    /*const response = await fetch('SOPManager/SOP/DisasterCategories');
    //    const disasterCategories = await response.json();

    //    if (disasterCategories) {
    //        const response2 = await fetch('SOPManager/SOP/NewStepMember');
    //        const stepMember = await response2.json();

    //        // 새로운 SOP를 생성하는 것이니 Version 정보는 초기화한다.
    //        this.removeVersion(disasterCategories);
    //        this.addStepMember(disasterCategories, stepMember);
    //        this.props.onSelectMenu(SopManager.Menu_New_SOP, disasterCategories);
    //    }*/
    //}

    /*initVersion(version) {
        if (version) {
            version.id = -1;
            version.versionName = this.newVersionName(version.versionName);
        }
        else {
            const now = this.getCurrentDateTime();

            version = {
                id: -1,
                isNormal: true,
                createTime: now,
                lastAccessTime: now,
                versionName: "V1.0",
                ownerID: -1,
                siteID: -1,
                description: null
            }
        }

        return version;
    }

    newVersionName(prevVersionName) {
        if (prevVersionName && prevVersionName.length > 1) {
            const versionTag = prevVersionName.substr(0, 1);
            const versionNumber = parseFloat(prevVersionName.substr(1));

            if (versionNumber) {
                return versionTag + (versionNumber + 0.1).toString();
            }
        }

        return "V1.0";
    }

    getCurrentDateTime() {
        const now = new Date();

        const year = now.getFullYear();
        const month = now.getMonth() + 1;
        const day = now.getDate();
        const hour = now.getHours();
        const min = now.getMinutes();
        const sec = now.getSeconds();

        const strYear = year.toString();
        const strMonth = month >= 10 ? month.toString() : "0" + month;
        const strDay = day >= 10 ? day.toString() : "0" + day;
        const strHour = hour >= 10 ? hour.toString() : "0" + hour;
        const strMin = min >= 10 ? min.toString() : "0" + min;
        const strSec = sec >= 10 ? sec.toString() : "0" + sec;

        return strYear + "-" + strMonth + "-" + strDay + "T" + strHour + ":" + strMin + ":" + strSec;
    }*/

    render() {
        return (
            <nav className="smMainNavMenus">
                <ul>
                    {
                        this.state.menuButtons.map((menu) =>
                        (
                            <SMMenuBtn key={menu.dataType} menu={menu} onClickMenu={this.onClickMenu} />
                        ))
                    }
                </ul>
            </nav>
        );
    }
}

export default SMMainNavMenu;