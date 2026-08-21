import React, { Component } from 'react';
import styles from './css/space.module.css';
/* import { FaHome } from "react-icons/fa"; */

export class SpaceMenus extends Component {
    static Menu_None = 0;
    static Menu_MoveTo_BuildingGroup = 12;
    static Menu_MoveTo_Site = 13;
    static Menu_MoveTo_Floor = 15;
    static Menu_Show_Outdoor = 16;
    static Menu_Show_Indoor = 17;

    static EditBasicInfo = "editBasicInfo";
    static EditSpaceInfo = "editSpaceInfo";
    static Upload3DModels = "upload3DModels";
    static Load3DModels = "load3DModels";
    static EditSensorList = "editSensorList";
    static EditEquipZoneCCTVs = "editEquipZoneCCTVs";
    static EditPois = "editPois";
    static EquipZoneNameText = "textEquipZoneName";
    static BuildingNameText = "textBuildingName";
    static BuildingGroupNameText = "textBuildingGroupName";

    constructor(props) {
        super(props);
        this.state = {
        }

        // 파일 업로드 버튼
        this.refXmlUpload = React.createRef();
        this.refXmlUploadTemp = React.createRef();

        this.onClickUploadDelegate = this.onClickUploadDelegate.bind(this);
    }

    componentDidMount() {
        const btnXmlUpload = this.refXmlUploadTemp.current;
        btnXmlUpload.addEventListener("click", this.onClickUploadDelegate);
    }

    componentWillUnmount() {
        const btnFileUpload = this.refXmlUploadTemp.current;
        btnFileUpload.addEventListener("click", this.onClickUploadDelegate);
    }

    // <input type='file'의 디자인 변경하려고 다른 버튼에 이벤트 붙임
    onClickUploadDelegate() {
        const galleryFile = this.refXmlUpload.current;

        const event = new MouseEvent("click", {
            bubbles: true,
            cancelable: true,
            view: window
        });

        galleryFile.dispatchEvent(event);
    }

    onClickMenu = (menu) => {
        this.props.onSelectMenu(menu);
    }

    getMenuItemClassName(isActive) {
        if (isActive) {
            return styles.menuItem + " " + styles.active;
        }

        return styles.menuItem;
    }

    render() {
        const biClass = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.EditBasicInfo);
        const siClass = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.EditSpaceInfo);
        const u3Class = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.Upload3DModels);
        const l3Class = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.Load3DModels);
        const slClass = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.EditSensorList);
        const ezClass = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.EditEquipZoneCCTVs);
        const epClass = this.getMenuItemClassName(this.props.selectedMenu === SpaceMenus.EditPois);

        return (
            <div className={styles.navBarMenu}>
                {/* <div className={styles.menuicon}><span>공간정보</span></div> */}
                <div className={styles.menuItems}>
                    <div className={biClass} onClick={() => this.onClickMenu(SpaceMenus.EditBasicInfo)}><span className={styles.basicInfoIcon}></span><span className={styles.basicInfoText}>기본정보 등록</span></div>
                    <div className={siClass} onClick={() => this.onClickMenu(SpaceMenus.EditSpaceInfo)}><span className={styles.areaInfoIcon}></span><span className={styles.areaInfoText}>공간정보 등록</span></div>
                    <div className={u3Class} onClick={() => this.onClickMenu(SpaceMenus.Upload3DModels)}><span className={styles.modelUploadIcon}></span><span className={styles.modelUploadText}>3D모델 업로드</span></div>
                    <div className={l3Class} onClick={() => this.onClickMenu(SpaceMenus.Load3DModels)}><span className={styles.modelViewIcon}></span><span className={styles.modelViewText}>3D모델뷰</span></div>
                    <div className={slClass} onClick={() => this.onClickMenu(SpaceMenus.EditSensorList)}><span className={styles.sensorListIcon}></span><span className={styles.sensorListText}>센서목록 편집</span></div>
                    <div className={ezClass} onClick={() => this.onClickMenu(SpaceMenus.EditEquipZoneCCTVs)}><span className={styles.equipZoneIcon}></span><span className={styles.equipZoneText}>구역별 CCTV 편집</span></div>
                    <div className={epClass} onClick={() => this.onClickMenu(SpaceMenus.EditPois)}><span className={styles.poiIcon}></span><span className={styles.poiText}>POI 편집</span></div>
                </div>
                <div className={styles.xmlArea}>
                    {/* <div className={styles.xmlIcon}></div> */}
                     <div className={styles.xmlText}><label htmlFor='openXML'>XML 불러오기</label></div>

                    {
                        //<input id='openXML' type='file' accept='.xml' onChange={(e) => this.props.onOpenXML(e)} className={styles.xmlLabel} />
                    }
                    <input type='file' style={{ display: "none" }} ref={this.refXmlUpload} onChange={(e) => this.props.onOpenXML(e)} />
                    <span className={styles.xmlUpload} ref={this.refXmlUploadTemp}>엑셀파일 업로드</span>

                  <input id='saveXML' type='button' value='XML 저장하기' onClick={() => this.props.onSaveXML(false)} className={styles.xmlSave}/>
                </div>
            </div>
        );
    }
}