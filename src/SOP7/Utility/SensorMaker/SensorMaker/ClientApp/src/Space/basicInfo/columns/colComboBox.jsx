import React, { Component } from 'react';
//import styles from '../../../Common/css/style.module.css';
//import uneStyles from '../../../Common/css/uneCommon.module.css';
import space from '../../css/space.module.css';

class ColComboBox extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            //id: null,            // 선택된 값
            //options: null,          // 콤보박스 리스트
            //isEditMode: false,      // 편집모드 체크값
            //isClickChk: false,      // 더블클릭 체크값
        };

        this.props = props;
        //this.state.id = this.props.id;
        //this.state.options = this.props.options;

        //if (this.state.id == null) {
        //    this.state.id = "";
        //}
    }

    componentDidUpdate(prevProps, prevState) {
        //if (prevProps.options !== this.props.options) {
        //    this.setState({ options: this.props.options });
        //}
    }

    onChangeCheck = (value) => {
        let sensor = this.props.sensor;
        if (this.props.valueType === 'sensorSubType') {            
            sensor.sensorSubType = Number(value);   
        }
        this.props.onChangeSensor(sensor);
    }

    getComboBoxItemUI() {
        let ui = [];
        let strName = null;
        if (this.props.options) {
            const optionCount = this.props.options.length;

            for (let i = 0; i < optionCount; i++) {
                const option = this.props.options[i];
                ui.push(<option key={option.id} value={option.id}>{option.name}</option>);

                if (option.id === this.props.value) {
                    strName = option.name;                    
                }
            }
        }

        return [ui, strName];
    }

    render() {
        let [ui, strName] = this.getComboBoxItemUI();

        return (
            <>
                {
                    (this.props.isEditMode)
                        ? 
                        <>
                            <select onChange={(e) => this.onChangeCheck(e.target.value)} defaultValue={this.props.id} autoFocus  className={space.sensorTypeSelect}>
                            {
                                ui
                            }
                            </select>
                        </>
                        :
                        <><span /*className={styles.fixation}*/>{strName}</span></>
                }
            </>
            );
    }
}

export default ColComboBox;