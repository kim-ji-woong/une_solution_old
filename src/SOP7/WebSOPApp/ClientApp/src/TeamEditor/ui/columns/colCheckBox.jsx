import React, { Component } from 'react';

import styles from '../../../Common/css/style.module.css';
import uneStyles from '../../../Common/css/uneCommon.module.css';

class ColCheckBox extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            value: null,
            isEditMode: false,
            check: false,
        };

        this.props = props;
        this.state.value = this.props.value;
        this.state.isEditMode = this.props.isEditMode;
        this.state.check = this.props.defaultChecked;
    }

    onChangeCheck = (e) => {
        let target = e;

        this.setState({ check: e.checked })
        this.props.onChange(e.checked);
        return;
    }


    render() {
        return (
            <td className={uneStyles.editCheckBox}><input type="checkbox" checked={this.state.check} onChange={(e) => this.onChangeCheck(e.target)} /></td>
        );
        //if (this.state.isEditMode) {
        //    return (
        //        <td className={uneStyles.editCheckBox}><input type="checkbox" checked={this.state.check} onChange={(e) => this.onChangeCheck(e.target)} /></td>
        //    );
        //}
        //else {            
        //    return (
        //        <td><span className={styles.fixation}>{this.state.value}</span></td>
        //    );
        //}
    }
}

export default ColCheckBox;