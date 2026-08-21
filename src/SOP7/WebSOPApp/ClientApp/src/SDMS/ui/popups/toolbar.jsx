import React, { Component } from 'react';
import styles from '../../../Common/css/newStyle.module.css';
import $ from 'jquery';
import SettingsStore from '../../../Settings/settingsStore';

class Toolbar extends Component {
    static keys = [];
    static shortcutKey = null;

    constructor(props) {
        super(props);

        SettingsStore.subscribe(function () {
            let data = SettingsStore.getState();

            if (data.actionType === 'SHORTCUT_KEY') {
                Toolbar.shortcutKey = data.shortcutKey;
            }
        }.bind(this));

        this.init();
    }

    init() {
        Toolbar.shortcutKey = SettingsStore.getState().shortcutKey;
    }

    componentDidMount() {
        // 단축키 이벤트 리스너
        window.addEventListener("keydown", (e) => this.keysPressed(e, this), false);
        window.addEventListener("keyup", this.keysReleased, false);
    }

    keysPressed(e, target) {
        // store an entry for every key pressed
        Toolbar.keys[e.keyCode] = true;

        // 단축키 설정 가져오기
        let shortcutKey = Toolbar.shortcutKey;

        if (shortcutKey === null || shortcutKey === undefined) {
            return;
        }

        if (Toolbar.keys[18] && Toolbar.keys[parseInt(shortcutKey.rotation)]) {
            // rotation 단축키
            console.log("rotation 단축키");
            target.props.startAutoRotation();

            Toolbar.keys[18] = false;
            Toolbar.keys[parseInt(shortcutKey.rotation)] = false;
            // prevent default browser behavior
            e.preventDefault();
        }
    }

    keysReleased(e) {
        // mark keys that were released
        Toolbar.keys[e.keyCode] = false;
    }

    onClickNavigator = (event) => {
        const btn = event.target;

        if (btn.classList.contains(styles.on)) {
            btn.classList.remove(styles.on);
            $(btn).next().slideUp();
        }
        else {
            $(btn).next().slideDown();
            btn.classList.add(styles.on);
        }
    }

    getAutoRotationOption() {
        if (this.props.useIdleTime) {
            return [styles.on, '자동회전 ON'];
        }

        return [styles.off, '자동회전 OFF'];
    }

    getFloorElements() {
        if (this.props.buildingID === null || this.props.floorDatas === null || this.props.floorDatas.length === 0) {
            return <></>
        }

        const floorDatas = [ ...this.props.floorDatas ];

        return (
            <ul className={styles.dsnFloor}>
                {
                    floorDatas.map((floorData, index) => {
                        if (floorData.length === 0 || floorData[0] === null) {
                            return <></>
                        }

                        if (floorData.length <= 2) {
                            return <li key={"floor_" + index}><a onClick={() => this.props.moveToFloor(this.props.buildingID, floorData[0])}>{floorData[1]}</a></li>
                        }

                        // 현재층
                        return <li key={"floor_" + index}><a className={styles.on} onClick={() => this.props.moveToFloor(this.props.buildingID, floorData[0])}>{floorData[1]}</a></li>
                    })
                }
            </ul>
            );
    }

    render() {
        const [autoRotationClassName, autoRotationText] = this.getAutoRotationOption();

        return (
            <div id={styles.dsNav}>
                <button onClick={this.onClickNavigator}>지도옵션 열기</button>
                <div>
                    <ul className={styles.dsnMenu}>
                        {/*
                        <li><a onClick={() => this.props.initViewport() }>초기화면</a></li>
                        <li><a onClick={() => this.props.setInitialViewport()}>기본뷰로 설정</a></li>
                        <li><a onClick={() => this.props.zoom(true)}>확대</a></li>
                        <li><a onClick={() => this.props.zoom(false)}>축소</a></li>
                        <li><a onClick={() => this.props.startAutoRotation() }>즉시회전</a></li>
                        <li><a className={autoRotationClassName} onClick={() => this.props.setUseIdleTime(!this.props.useIdleTime)}>{autoRotationText}</a></li>
                        */}
                        <li><a onClick={() => this.props.initViewport()}></a></li>
                        <li><a onClick={() => this.props.setInitialViewport()}> </a></li>
                        <li><a onClick={() => this.props.zoom(true)}> </a></li>
                        <li><a onClick={() => this.props.zoom(false)}> </a></li>
                        <li><a onClick={() => this.props.startAutoRotation()}> </a></li>
                        <li><a className={autoRotationClassName} onClick={() => this.props.setUseIdleTime(!this.props.useIdleTime)}> </a></li>
                    </ul>

                    {
                        this.getFloorElements()
                    }
                </div>
            </div>
        );
    }
}


export default Toolbar;