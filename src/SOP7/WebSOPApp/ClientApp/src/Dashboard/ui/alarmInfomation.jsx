import React, { Component } from 'react';
import $ from 'jquery';

import dashboard from '../css/dashboardNew.module.css';
import SDMSResource from '../../SDMS/resource/id';
import DashboardResource from '../resource/id';
import ProjectResource from '../../Root/resource/id';

class AlarmInfomation extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }


    getAlarmCount = () => {
        if (this.props.selectWeeklyAlarms === null || this.props.selectWeeklyAlarms === undefined)
            return [0, 0, 0, 0, 0];

        let selectWeeklyAlarms = this.props.selectWeeklyAlarms;
        const selectSensors = this.props.selectSensors;
        let fireCount = 0;
        let iotCount = 0;
        let psmCount = 0;
        let etcCount = 0;
        let safetyCount = 0;
        let svmsCount = 0;

        let safetyCCTVs = [];

        // .TODO: safety 카운팅
        if (selectSensors !== null && selectSensors !== undefined) {
            const cctvs = selectSensors.cctvs;
            
            for (let i = 0; i < cctvs.length; i++) {
                const cctv = cctvs[i];

                if (cctv.type === "SAFETY-I")
                    safetyCCTVs.push(cctv);
            }
        }

        for (let i = 0; i < selectWeeklyAlarms.length; i++) {
            let alarm = selectWeeklyAlarms[i];
            let facilityType = alarm.facilityType;

            if (facilityType === SDMSResource.facilityType.FIRE) {
                fireCount++;
            } else if (SDMSResource.isSVMSSensorType(facilityType)) {
                svmsCount++;

                for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                    let alarm = selectWeeklyAlarms[i];

                    if (SDMSResource.isSVMSSensorType(alarm.facilityType)) {

                        for (let j = 0; j < safetyCCTVs.length; j++) {
                            const cctv = safetyCCTVs[j];

                            if (cctv.id === alarm.orgSensorID) {
                                safetyCount++;
                                break;
                            }
                        }
                    }
                }

            } else if (SDMSResource.isPSMSensorType(facilityType)) {
                //iotCount++;
                psmCount++;

                // .TODO: safety 카운팅
            } else if (SDMSResource.isETCSensorType(facilityType)) {
                //iotCount++;
                etcCount++;

                // .TODO: safety 카운팅
            }
        }

        return [fireCount, svmsCount, psmCount, etcCount, safetyCount];
     
    }

    getAlarmBoard() {
        const alarmList = this.getAlarmList();

        let alarmBoard = [];

        let pageNum = alarmList.length / 6;
        pageNum = Math.ceil(pageNum);

        //// 페이징 ui
        //alarmBoard.push(<div className={dashboard.nextCircle}>);

        if (pageNum > 0) {
            for (let i = 0; i < pageNum; i++) {
                if (i === 0) {
                    alarmBoard.push(<span className={dashboard.miniCircle + " " + dashboard.miniCircleAct}></span>);
                } else {
                    alarmBoard.push(<span className={dashboard.miniCircle}></span>);
                }
            }
        }

    }

    getAlarmList = () => {
        // 타입별 항목 가져오기
        const type = this.props.type;
        const selectWeeklyAlarms = this.props.selectWeeklyAlarms;
        const selectSensors = this.props.selectSensors;
        const materials = this.props.materials;

        const selectDay = this.props.selectDay;
        //const sensorZoneHistorys = this.props.sensorZoneHistorys;

        let alarmList = [];
        let materialList = [];

        if (selectSensors === null || selectSensors === undefined)
            return alarmList;

        let tempList = [];
        let maxCount = 0;

        if (type === DashboardResource.displayInfoType.FIRE) {

            if (selectSensors.fireSensors === null || selectSensors.fireSensors === undefined)
                return alarmList;

            const fireSensors = selectSensors.fireSensors;

            let normalCount = 0;
            let smokeCount = 0;
            let flameCount = 0;
            let heatCount = 0;

            for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                let alarm = selectWeeklyAlarms[i];

                if (alarm.facilityType === SDMSResource.facilityType.FIRE) {
                    for (let j = 0; j < fireSensors.length; j++) {
                        let fireSensor = fireSensors[j];

                        if (alarm.orgSensorID === fireSensor.id) {

                            let fireType = fireSensor.sensorSubType;

                            if (fireType === DashboardResource.fireSubType.HEAT)
                                heatCount++;
                            else if (fireType === DashboardResource.fireSubType.FLAME)
                                flameCount++;
                            else if (fireType === DashboardResource.fireSubType.SMOKE)
                                smokeCount++;
                            else
                                normalCount++;

                            break;
                        }
                    }
                }
            }

            if (normalCount > 0)
                tempList.push({ typeName: "일반", typeValue: normalCount });
            if (heatCount > 0)
                tempList.push({ typeName: "열", typeValue: heatCount });
            if (smokeCount > 0)
                tempList.push({ typeName: "연기", typeValue: smokeCount });
            if (flameCount > 0)
                tempList.push({ typeName: "불꽃", typeValue: flameCount });

            for (let i = 0; i < tempList.length; i++) {
                let data = tempList[i];

                if (data.typeValue === 0 || alarmList.length === 0) {
                    alarmList.push(data);

                    if (maxCount < data.typeValue)
                        maxCount = data.typeValue;
                } else if (maxCount < data.typeValue) {
                    maxCount = data.typeValue;
                    alarmList.unshift(data);
                } else {
                    let chk = false;

                    for (let j = 0; j < alarmList.length; j++) {
                        let temp = alarmList[j];

                        if (temp.typeValue < data.typeValue) {
                            alarmList.splice(j, 0, data);
                            chk = true;
                            break;
                        }
                    }

                    if (chk === false)
                        alarmList.push(data);
                }
            }

        } else if (type === DashboardResource.displayInfoType.INTELLIGENT) {
            if (selectSensors.cctvs === null || selectSensors.cctvs === undefined)
                return alarmList;

            let invasion = 0;   // 침입
            let loiter = 0;      // 배회
            let collapse = 0;       // 넘어짐
            let theft = 0;      // 도난
            let neglect = 0;    // 방치
            let fence = 0;      // 가상 펜스
            let fire = 0;       // 화재

            for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                let alarm = selectWeeklyAlarms[i];

                if (SDMSResource.isSVMSSensorType(alarm.facilityType)) {

                    if (alarm.facilityType === SDMSResource.facilityType.Intrusion_S1) {
                        invasion++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.Loiter_S1) {
                        loiter++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.Collapse_S1) {
                        collapse++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.Theft_S1) {
                        theft++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.Neglect_S1) {
                        neglect++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.VirtualFence_S1) {
                        fence++;
                    } else if (alarm.facilityType === SDMSResource.facilityType.Fire_S1) {
                        fire++;
                    }
                }
            }

            if (invasion > 0)
                tempList.push({ typeName: "침입", typeValue: invasion });
            if (loiter > 0)
                tempList.push({ typeName: "배회", typeValue: loiter });
            if (collapse > 0)
                tempList.push({ typeName: "넘어짐", typeValue: collapse });
            if (theft > 0)
                tempList.push({ typeName: "도난", typeValue: theft });
            if (neglect > 0)
                tempList.push({ typeName: "방치", typeValue: neglect });
            if (fence > 0)
                tempList.push({ typeName: "가상 펜스", typeValue: fence });
            if (fire > 0)
                tempList.push({ typeName: "화재", typeValue: fire });

            for (let i = 0; i < tempList.length; i++) {
                let data = tempList[i];

                if (data.typeValue === 0 || alarmList.length === 0) {
                    alarmList.push(data);

                    if (maxCount < data.typeValue)
                        maxCount = data.typeValue;
                } else if (maxCount < data.typeValue) {
                    maxCount = data.typeValue;
                    alarmList.unshift(data);
                } else {
                    let chk = false;

                    for (let j = 0; j < alarmList.length; j++) {
                        let temp = alarmList[j];

                        if (temp.typeValue < data.typeValue) {
                            alarmList.splice(j, 0, data);
                            chk = true;
                            break;
                        }
                    }

                    if (chk === false)
                        alarmList.push(data);
                }
            }

        } else if (type === DashboardResource.displayInfoType.ETC) {
            /*
            let Temp = 0;
            let Humi = 0;
            let CO2 = 0;
            let TVOC = 0;
            let Dust_PM1 = 0;
            let Dust_PM2 = 0;
            let Dust_PM10 = 0;
            let AirPress = 0;
            let Inclin_X = 0;
            let Inclin_Y = 0;
            let Vib_X = 0;
            let Vib_Y = 0;
            let Vib_Z = 0;
            let Noise = 0;
            let BLE_Count = 0;
            let O2 = 0;
            let Value = 0;
            let mA = 0;
            let Contact = 0;
            let Relay = 0;
            */

            for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                let alarm = selectWeeklyAlarms[i];
                const nType = alarm.facilityType;
                let materialType = alarm.materialType;

                if (SDMSResource.isETCSensorType(nType)) {
                    if (materialType === undefined)
                        materialType = null;

                    if (materialList[materialType] === null || materialList[materialType] === undefined)
                        materialList[materialType] = 1;
                    else {
                        let num = materialList[materialType];
                        materialList[materialType] = num + 1;
                    }

                    /*
                    if (nType === SDMSResource.facilityType.Temp)
                        Temp++;
                    else if (nType === SDMSResource.facilityType.Humi)
                        Humi++;
                    else if (nType === SDMSResource.facilityType.CO2)
                        CO2++;
                    else if (nType === SDMSResource.facilityType.TVOC)
                        TVOC++;
                    else if (nType === SDMSResource.facilityType.Dust_PM1)
                        Dust_PM1++;
                    else if (nType === SDMSResource.facilityType.Dust_PM2)
                        Dust_PM2++;
                    else if (nType === SDMSResource.facilityType.Dust_PM10)
                        Dust_PM10++;
                    else if (nType === SDMSResource.facilityType.AirPress)
                        AirPress++;
                    else if (nType === SDMSResource.facilityType.Inclin_X)
                        Inclin_X++;
                    else if (nType === SDMSResource.facilityType.Inclin_Y)
                        Inclin_Y++;
                    else if (nType === SDMSResource.facilityType.Vib_X)
                        Vib_X++;
                    else if (nType === SDMSResource.facilityType.Vib_Y)
                        Vib_Y++;
                    else if (nType === SDMSResource.facilityType.Vib_Z)
                        Vib_Z++;
                    else if (nType === SDMSResource.facilityType.Noise)
                        Noise++;
                    else if (nType === SDMSResource.facilityType.BLE_Count)
                        BLE_Count++;
                    else if (nType === SDMSResource.facilityType.O2)
                        O2++;
                    else if (nType === SDMSResource.facilityType.Value)
                        Value++;
                    else if (nType === SDMSResource.facilityType.mA)
                        mA++;
                    else if (nType === SDMSResource.facilityType.Contact)
                        Contact++;
                    else if (nType === SDMSResource.facilityType.Relay)
                        Relay++;
                    */
                }
            }

            if (materials !== null && materials !== undefined) {
                for (let key in materialList) {

                    if (key === "null") {
                        tempList.push({ typeName: "기타", typeValue: materialList[key] });
                        continue;
                    }

                    for (let i = 0; i < materials.length; i++) {
                        let material = materials[i];

                        if (material.id.toString() === key) {
                            tempList.push({ typeName: material.materialName, typeValue: materialList[key] });
                            break;
                        }
                    }
                }
            }

            /*
            tempList.push({ typeName: "온도", typeValue: Temp });
            tempList.push({ typeName: "습도", typeValue: Humi });
            tempList.push({ typeName: "이산화탄소", typeValue: CO2 });
            tempList.push({ typeName: "TVOC", typeValue: TVOC });
            tempList.push({ typeName: "미세먼지(PM 1.0)", typeValue: Dust_PM1 });
            tempList.push({ typeName: "미세먼지(PM 2.5)", typeValue: Dust_PM2 });
            tempList.push({ typeName: "미세먼지(PM 10)", typeValue: Dust_PM10 });
            tempList.push({ typeName: "기압", typeValue: AirPress });
            tempList.push({ typeName: "기울기(X)", typeValue: Inclin_X });
            tempList.push({ typeName: "기울기(Y)", typeValue: Inclin_Y });
            tempList.push({ typeName: "진동(X)", typeValue: Vib_X });
            tempList.push({ typeName: "진동(Y)", typeValue: Vib_Y });
            tempList.push({ typeName: "진동(Z)", typeValue: Vib_Z });
            tempList.push({ typeName: "소음", typeValue: Noise });
            tempList.push({ typeName: "BLE Count", typeValue: BLE_Count });
            tempList.push({ typeName: "산소", typeValue: O2 });
            tempList.push({ typeName: "ESH_v5.1 측정값", typeValue: Value });
            tempList.push({ typeName: "mA", typeValue: mA });
            tempList.push({ typeName: "접점", typeValue: Contact });
            tempList.push({ typeName: "릴레이", typeValue: Relay });
            */

            for (let i = 0; i < tempList.length; i++) {
                let data = tempList[i];

                if (data.typeValue === 0 || alarmList.length === 0) {
                    alarmList.push(data);

                    if (maxCount < data.typeValue)
                        maxCount = data.typeValue;
                } else if (maxCount < data.typeValue) {
                    maxCount = data.typeValue;
                    alarmList.unshift(data);
                } else {
                    let chk = false;

                    for (let j = 0; j < alarmList.length; j++) {
                        let temp = alarmList[j];

                        if (temp.typeValue < data.typeValue) {
                            alarmList.splice(j, 0, data);
                            chk = true;
                            break;
                        }
                    }

                    if (chk === false)
                        alarmList.push(data);
                }
            }

        } else if (type === DashboardResource.displayInfoType.SAFETY_EYE) {

            // .TODO: safety 카운팅
            const cctvs = selectSensors.cctvs;
            let safetyCCTVs = [];
            let safetyCCTV1 = 0;
            let safetyCCTV2 = 0;

            for (let i = 0; i < cctvs.length; i++) {
                const cctv = cctvs[i];

                if (cctv.type === "SAFETY-1")
                    safetyCCTVs.push(cctv);
            }

            for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                let alarm = selectWeeklyAlarms[i];

                if (SDMSResource.isSVMSSensorType(alarm.facilityType)) {

                    for (let j = 0; j < safetyCCTVs.length; j++) {
                        const cctv = safetyCCTVs[j];

                        if (cctv.id === alarm.orgSensorID) {
                            if (j === 0)
                                safetyCCTV1++;
                            else
                                safetyCCTV2++;

                            break;
                        }
                    }

                }
            }

            alarmList.push({ typeName: "장비1-가스", typeValue: 0 });
            alarmList.push({ typeName: "장비1-지능형 영상감지", typeValue: safetyCCTV1 });
            alarmList.push({ typeName: "장비2-가스", typeValue: 0 });
            alarmList.push({ typeName: "장비2-지능형 영상감지", typeValue: safetyCCTV2 });

        } else if (type === DashboardResource.displayInfoType.PSM) {

            // 사이트 별로 구별 필요
            const siteID = ProjectResource.SiteID;

            /*
            // 솔브레인
            let HF = 0;
            let CO = 0;
            let HCL = 0;
            let CH3C = 0;
            let N2H4 = 0;
            let CA = 0;
            let EA = 0;
            let VOC = 0;
            let H2O2 = 0;
            let THC = 0;
            let HNO3 = 0;
            let CL = 0;
            let TOLUENE = 0;
            let F2 = 0;
            let NH3 = 0;
            let LNG = 0;
            let PGMEA = 0;
            let H2S = 0;

            // 녹십자
            let temp = 0;
            */

            for (let i = 0; i < selectWeeklyAlarms.length; i++) {
                let alarm = selectWeeklyAlarms[i];
                const nType = alarm.facilityType;
                let materialType = alarm.materialType;

                if (SDMSResource.isPSMSensorType(nType)) {
                    if (materialType === undefined)
                        materialType = null;

                    if (materialList[materialType] === null || materialList[materialType] === undefined)
                        materialList[materialType] = 1;
                    else {
                        let num = materialList[materialType];
                        materialList[materialType] = num + 1;
                    }

                    /*
                    // 솔브레인
                    if (nType === SDMSResource.facilityType.HF)
                        HF++;
                    else if (nType === SDMSResource.facilityType.CO)
                        CO++;
                    else if (nType === SDMSResource.facilityType.HCL)
                        HCL++;
                    else if (nType === SDMSResource.facilityType.CH3C)
                        CH3C++;
                    else if (nType === SDMSResource.facilityType.N2H4)
                        N2H4++;
                    else if (nType === SDMSResource.facilityType.CA)
                        CA++;
                    else if (nType === SDMSResource.facilityType.EA)
                        EA++;
                    else if (nType === SDMSResource.facilityType.VOC)
                        VOC++;
                    else if (nType === SDMSResource.facilityType.H2O2)
                        H2O2++;
                    else if (nType === SDMSResource.facilityType.THC)
                        THC++;
                    else if (nType === SDMSResource.facilityType.HNO3)
                        HNO3++;
                    else if (nType === SDMSResource.facilityType.CL)
                        CL++;
                    else if (nType === SDMSResource.facilityType.TOLUENE)
                        TOLUENE++;
                    else if (nType === SDMSResource.facilityType.F2)
                        F2++;
                    else if (nType === SDMSResource.facilityType.NH3)
                        NH3++;
                    else if (nType === SDMSResource.facilityType.LNG)
                        LNG++;
                    else if (nType === SDMSResource.facilityType.PGMEA)
                        PGMEA++;
                    else if (nType === SDMSResource.facilityType.H2S)
                        H2S++;
                    */

                }
            }

            /*
            // 솔브레인
            if (siteID === 10) {
                tempList.push({ typeName: "불화수소", typeValue: HF });
                tempList.push({ typeName: "일산화탄소", typeValue: CO });
                tempList.push({ typeName: "염화수소", typeValue: HCL });
                tempList.push({ typeName: "초산", typeValue: CH3C });
                tempList.push({ typeName: "하이드라진", typeValue: N2H4 });
                tempList.push({ typeName: "CA Gas", typeValue: CA });
                tempList.push({ typeName: "에틸알콜", typeValue: EA });
                tempList.push({ typeName: "VOC", typeValue: VOC });
                tempList.push({ typeName: "과수", typeValue: H2O2 });
                tempList.push({ typeName: "에탄올", typeValue: THC });
                tempList.push({ typeName: "질산", typeValue: HNO3 });
                tempList.push({ typeName: "염소가스", typeValue: CL });
                tempList.push({ typeName: "톨루엔", typeValue: TOLUENE });
                tempList.push({ typeName: "불소", typeValue: F2 });
                tempList.push({ typeName: "암모니아", typeValue: NH3 });
                tempList.push({ typeName: "액화천연가스", typeValue: LNG });
                tempList.push({ typeName: "유기가스", typeValue: PGMEA });
                tempList.push({ typeName: "황화수소", typeValue: H2S });
            } else {
                // 녹십자
                tempList.push({ typeName: "가스", typeValue: temp });
            }
            */

            if (materials !== null && materials !== undefined) {
                for (let key in materialList) {

                    if (key === "null") {
                        tempList.push({ typeName: "유해화학물질", typeValue: materialList[key] });
                        continue;
                    }

                    for (let i = 0; i < materials.length; i++) {
                        let material = materials[i];

                        if (material.id.toString() === key) {
                            tempList.push({ typeName: material.materialName, typeValue: materialList[key] });
                            break;
                        }
                    }
                }
            }
            
            for (let i = 0; i < tempList.length; i++) {
                let data = tempList[i];

                if (data.typeValue === 0 || alarmList.length === 0) {
                    alarmList.push(data);

                    if (maxCount < data.typeValue)
                        maxCount = data.typeValue;
                } else if (maxCount < data.typeValue) {
                    maxCount = data.typeValue;
                    alarmList.unshift(data);
                } else {
                    let chk = false;

                    for (let j = 0; j < alarmList.length; j++) {
                        let temp = alarmList[j];

                        if (temp.typeValue < data.typeValue) {
                            alarmList.splice(j, 0, data);
                            chk = true;
                            break;
                        }
                    }

                    if (chk === false)
                        alarmList.push(data);
                }
            }
        }

        let alarmListUI = [];

        for (let i = 0; i < alarmList.length; i++) {
            let alarm = alarmList[i];

            alarmListUI.push(<p key={"alarmListUI_" + i}>{alarm.typeName}<span>{alarm.typeValue}</span></p>);
        }

        return alarmListUI;
    }

    getTypeBtnClass = () => {
        const type = this.props.type;
        let fireClass = "";
        let svmsClass = "";
        let safetyClass = "";
        let psmClass = "";
        let etcClass = "";

        if (type === DashboardResource.displayInfoType.FIRE) {
            fireClass = dashboard.typeAct;
        } else if (type === DashboardResource.displayInfoType.INTELLIGENT) {
            svmsClass = dashboard.typeAct;
        } else if (type === DashboardResource.displayInfoType.SAFETY_EYE) {
            safetyClass = dashboard.typeAct;
        } else if (type === DashboardResource.displayInfoType.PSM) {
            psmClass = dashboard.typeAct;
        } else if (type === DashboardResource.displayInfoType.ETC) {
            etcClass = dashboard.typeAct;
        }

        return [fireClass, svmsClass, safetyClass, psmClass, etcClass];
    }

    componentDidMount() {

    }

    displaySiteUI = () => {
        const siteID = ProjectResource.SiteID;
        let displaySiteUI = [];
        //const [fireCount, svmsCount, iotCount, safetyCount] = this.getAlarmCount();
        const [fireCount, svmsCount, psmCount, etcCount, safetyCount] = this.getAlarmCount();
        const alarmListUI = this.getAlarmList();
        const [fireClass, svmsClass, safetyClass, psmClass, etcClass] = this.getTypeBtnClass();

        if (siteID === ProjectResource.Site.GCC) {
            /* 녹십자 */
            displaySiteUI.push(
                <div className={dashboard.alarmInfomationGC}>
                    <div className={dashboard.alarmInTitle}>이상 센서 알람</div>
                    <div className={dashboard.alarmFlexGC}>
                        <ul>
                            <li className={fireClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.FIRE)}><div className={dashboard.blueSquare}><span className={dashboard.typeFire}></span></div><span className={dashboard.fireTitle}>화재</span><div className={dashboard.alarmInfo1}>{fireCount}<span>건</span></div></li>
                            <li className={psmClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.PSM)}><div className={dashboard.blueSquare}><span className={dashboard.typeIOT}></span></div><span className={dashboard.iotTitle}>누출</span><div className={dashboard.alarmInfo4}>{psmCount}<span>건</span></div></li>
                            <li className={svmsClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.INTELLIGENT)}><div className={dashboard.blueSquare}><span className={dashboard.typeCCTV}></span></div><span className={dashboard.cctvTitle}>CCTV</span><div className={dashboard.alarmInfo2}>{svmsCount}<span>건</span></div></li>
                        </ul>
                    </div>
                    <div className={dashboard.alarmListTitle}>감지항목별 알림현황 (단위: 건)</div>
                    <div className={dashboard.alarmcategoryGC + " " + dashboard.scrollbar}>
                        <div className={dashboard.alarmList}>
                            {alarmListUI}
                        </div>
                    </div>
                </div>
            );
        } else {
            /* 솔브레인 */
            displaySiteUI.push(
                <div className={dashboard.alarmInfomation}>
                    <div className={dashboard.alarmInTitle}>이상 센서 알람</div>
                    <div className={dashboard.alarmFlex}>
                        <ul>
                            <li className={fireClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.FIRE)}><div className={dashboard.blueSquare}><span className={dashboard.typeFire}></span></div><span className={dashboard.fireTitle}>화재</span><div className={dashboard.alarmInfo1}>{fireCount}<span>건</span></div></li>
                            <li className={psmClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.PSM)}><div className={dashboard.blueSquare}><span className={dashboard.typeIOT}></span></div><span className={dashboard.iotTitle}>누출</span><div className={dashboard.alarmInfo4}>{psmCount}<span>건</span></div></li>
                            <li className={etcClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.ETC)}><div className={dashboard.blueSquare}><span className={dashboard.typeETC}></span></div><span className={dashboard.etcTitle}>ETC</span><div className={dashboard.alarmInfo5}>{etcCount}<span>건</span></div></li>
                            <li className={svmsClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.INTELLIGENT)}><div className={dashboard.blueSquare}><span className={dashboard.typeCCTV}></span></div><span className={dashboard.cctvTitle}>CCTV</span><div className={dashboard.alarmInfo2}>{svmsCount}<span>건</span></div></li>
                            <li className={safetyClass} onClick={() => this.props.changeType(DashboardResource.displayInfoType.SAFETY_EYE)}><div className={dashboard.blueSquare}><span className={dashboard.typeSafety}></span></div><span className={dashboard.safetyTitle}>S.I</span><div className={dashboard.alarmInfo3}>{safetyCount}<span>건</span></div></li>
                        </ul>
                    </div>
                    <div className={dashboard.alarmListTitle}>감지항목별 알림현황 (단위: 건)</div>
                    <div className={dashboard.alarmcategory + " " + dashboard.scrollbar}>
                        <div className={dashboard.alarmList}>
                            {alarmListUI}
                        </div>
                    </div>
                    <div onClick={() => this.props.changeMode(DashboardResource.mode.sub)} className={dashboard.listButton}>상세 현황 모두 보기</div>
                </div>
            );
        }

        return displaySiteUI;
    }

    render() {
        const displaySiteUI = this.displaySiteUI();

        return (
            <>
                {   /* 사이트별 UI */
                    displaySiteUI
                }
            </>
        );
    }
}
export default AlarmInfomation;