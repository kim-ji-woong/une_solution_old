import React, { Component } from 'react';
import uis from '../../Common/css/ui.module.css';
import newStyles from '../../Common/css/newStyle.module.css';
import $ from 'jquery';
import SopSimulatorResource from "../resource/id";

class SopSimulatorMenuBtn extends Component {
    constructor(props) {
        super(props);

        this.props = props;
    }

    onClickMenu(menu) {
        if (this.props.menuEvent.handler) {
            this.props.menuEvent.handler(menu);
        }
    }

    getParameter(menu) {
        return "";
    }

    render() {
        return (
            /*
            <div className={"btnCallSOP " +uis.fileWrap}>
                <button type="button" className={uis.btnFile} onClick={() => this.onClickMenu(SopSimulatorResource.ID.menu.callSOP)}><i className={uis.iconSOP}></i></button>
            </div> 
            */
            <div className={newStyles.rqQck + " rqBtn"}>
                <button className="rqQckBtn" onClick={() => this.onClickMenu(SopSimulatorResource.ID.menu.callSOP)}>메뉴열기</button>
            </div>
        );
    }

}

export default SopSimulatorMenuBtn;