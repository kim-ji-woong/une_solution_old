import React, { Component } from 'react';
import '../Root/css/custom.css';
import Contents3D from './contents3D';
import { Menus } from './menus';

export class BodyManager extends Component {
    constructor(props) {
        super(props);
        this.state =
        {
            selectedMenu: Menus.View_All_Outside,
            //selectedMenu: Menus.View_1_1_All,
            selectedPOIMenu: null
            //selectedMenu: Menus.View_1_1_1F
        };
    }

    onSelectMenu = (menu) => {
        if (menu === Menus.CUP_BLUE || menu === Menus.CUP_WHITE) {
            if (this.state.selectedPOIMenu !== menu) {
                this.setState({ selectedPOIMenu: menu });
            }
            else {
                this.setState({ selectedPOIMenu: null });
            }
        }
        else {
            if (this.state.selectedMenu !== menu) {
                this.setState({ selectedMenu: menu });
            }
        }
    }

    render() {
        return (
            <div className="bodyArea">
                <Menus onSelectMenu={this.onSelectMenu} selectedMenu={this.state.selectedMenu} poi={this.state.selectedPOIMenu}/>
                <Contents3D contents={this.state.selectedMenu} poi={this.state.selectedPOIMenu} onSelectContents={this.onSelectMenu} />
            </div>
        );
    }
}
