import React, { Component } from 'react';
import styles from '../css/spatial.module.css';
import { SpaceDataManager } from '../services/spaceDataManager';

export class PoiHeight extends Component {
    constructor(props) {
        super(props);
        this.state = {
            sensorTypeHeight: { ...props.sensorTypeHeight },
            prevInstance: this,
            prevProps: props,
            firePoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.FireSensorType, props.sensorTypeHeight),
            psmPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.PSMSensorType, props.sensorTypeHeight),
            etcPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.EtcSensorType, props.sensorTypeHeight),
            cctvPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.CCTVType, props.sensorTypeHeight)
        }
    }

    static getDerivedStateFromProps(props, state) {
        if (props === state.prevProps) {
            return state;
        }

        return {
            sensorTypeHeight: {...props.sensorTypeHeight},
            prevInstance: state.prevInstance,
            prevProps: props,
            firePoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.FireSensorType, props.sensorTypeHeight),
            psmPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.PSMSensorType, props.sensorTypeHeight),
            etcPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.EtcSensorType, props.sensorTypeHeight),
            cctvPoiHeight: PoiHeight._getSensorTypeHeight(SpaceDataManager.CCTVType, props.sensorTypeHeight)
        };
    }

    /*getSensorTypeHeight(sensorType) {
        const sensorTypeHeight = this.props.sensorTypeHeight;

        if (sensorTypeHeight) {
            const height = sensorTypeHeight[sensorType];

            if (height !== null && height !== undefined) {
                return height.toFixed(2);
            }
        }

        return "";
    }*/

    getSensorTypeHeight(sensorType) {
        return PoiHeight._getSensorTypeHeight(sensorType, this.state.sensorTypeHeight);
    }

    static _getSensorTypeHeight(sensorType, sensorTypeHeight) {
        if (sensorTypeHeight) {
            return sensorTypeHeight[sensorType];
        }

        return null;
    }

    onChangeText(e, sensorType) {
        const elevation = parseFloat(e.target.value);

        if (elevation !== null && elevation !== undefined && isNaN(elevation) === false) {
            const sensorTypeHeight = { ...this.state.sensorTypeHeight };
            sensorTypeHeight[sensorType] = elevation;

            if (sensorType === SpaceDataManager.FireSensorType) {
                this.setState({ sensorTypeHeight, firePoiHeight: e.target.value });
            }
            else if (sensorType === SpaceDataManager.PSMSensorType) {
                this.setState({ sensorTypeHeight, psmPoiHeight: e.target.value });
            }
            else if (sensorType === SpaceDataManager.EtcSensorType) {
                this.setState({ sensorTypeHeight, etcPoiHeight: e.target.value });
            }
            else if (sensorType === SpaceDataManager.CCTVType) {
                this.setState({ sensorTypeHeight, cctvPoiHeight: e.target.value });
            }
        }
    }

    render() {
        return (
            <div className={styles.poiHeightBody}>
                <span className={styles.poiHeightItem}>화재센서
                    <span className={styles.poiHeightItemArea}>
                        <input className={styles.poiHeightText} type="text" value={this.state.firePoiHeight} onChange={(e) => this.onChangeText(e, SpaceDataManager.FireSensorType)} />
                    </span>
                </span>
                <span className={styles.poiHeightItem}>누출센서
                    <span className={styles.poiHeightItemArea}>
                        <input className={styles.poiHeightText} type="text" value={this.state.psmPoiHeight} onChange={(e) => this.onChangeText(e, SpaceDataManager.PSMSensorType)} />
                    </span>
                </span>
                <span className={styles.poiHeightItem}>기타센서
                    <span className={styles.poiHeightItemArea}>
                        <input className={styles.poiHeightText} type="text" value={this.state.etcPoiHeight} onChange={(e) => this.onChangeText(e, SpaceDataManager.EtcSensorType)} />
                    </span>
                </span>
                <span className={styles.poiHeightItem}>CCTV
                    <span className={styles.poiHeightItemArea}>
                        <input className={styles.poiHeightText} type="text" value={this.state.cctvPoiHeight} onChange={(e) => this.onChangeText(e, SpaceDataManager.CCTVType)} />
                    </span>
                </span>
                <button className={styles.poiHeightButton} onClick={() => this.props.onChangeSensorTypeHeight(this.state.sensorTypeHeight)}>적용</button>
            </div>
        );
    }
}