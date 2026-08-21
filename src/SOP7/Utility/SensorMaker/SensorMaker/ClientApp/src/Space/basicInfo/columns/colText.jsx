import React, { Component } from 'react';
import space from './../../css/space.module.css';

class ColText extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            value: null,
        };

        this.props = props;
        this.state.value = this.props.value;
    }

    //정규식
    onBlurCheck = (value) => {        
        let sensor = this.props.sensor;

        if (this.props.valueType === 'name') {
            if (sensor.name === value) {
                return;
            }
            sensor.name = value;
        }
        else if (this.props.valueType === 'uniqueKey') {
            if (sensor.uniqueKey === value) {
                return;
            }
            sensor.uniqueKey = value;
        }
        else if (this.props.valueType === 'tagNo') {
            if (sensor.tagNo === Number(value)) {
                return;
            }
            sensor.tagNo = Number(value);
        }
        else if (this.props.valueType === 'positionName') {
            if (sensor.positionName === value) {
                return;
            }
            sensor.positionName = value;
        }
        this.props.onChangeSensor(value); // 포커스 잃을 경우에만 부모에게 값을 전달
    }

    onChangeCheck = (value) => {
        this.setState({ value: value });
    }

    handleKeyPress = (e) => {
        if (e.key === "Enter") {
            this.onBlurCheck(e.target.value);
        }
    }

    render() {
        
        return (
            <> 
                {
                    (this.props.isEditMode)
                        ?
                        <input type="text"
                            //id={colID}
                            onChange={(e) => this.onChangeCheck(e.target.value)}
                            onBlur={(e) => this.onBlurCheck(e.target.value)}
                            onKeyPress={this.handleKeyPress}
                            value={this.state.value || ''}
                        />
                        :
                        <span>{this.state.value}</span>
                }
            </>
            );
    }
}

export default ColText;