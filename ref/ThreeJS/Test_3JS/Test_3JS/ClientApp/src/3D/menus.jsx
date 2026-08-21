import React, { Component } from 'react';
import styles from './css/_3d.module.css';
import { FaHome } from "react-icons/fa";


export class Menus extends Component {
    static View_1_1_1F = "01_soubrain_1-1-1f.glb";
    static View_1_1_2F = "02_soubrain_1-1-2f.glb";
    static View_out = "03_soubrain_out.glb";
    static View_1_1_All = "07_soubrain_T1_all.glb";
    static View_1_2_1F = "04_soubrain_1-2-1f.glb";
    static View_1_2_2F = "05_soubrain_1-2-2f.glb";
    static View_100_contents = "01_soubrain_100_test_all.glb";
    static View_All_Outside = "01_soubr_out_test_all-01.glb";

    static CUP_WHITE = "cup_white.png";
    static CUP_BLUE = "cup_blue.png";

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
        return (
            <div className={styles.navBarMenu}>
                <div className={styles.menuicon}><FaHome size="40" /><span>솔브레인</span></div>
                <div className={styles.menuItems}>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_1_1_1F)} onClick={() => this.onClickMenu(Menus.View_1_1_1F)}>1-1동 1층</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_1_1_2F)} onClick={() => this.onClickMenu(Menus.View_1_1_2F)}>1-1동 2층</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_out)} onClick={() => this.onClickMenu(Menus.View_out)}>솔브레인 외부</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_1_1_All)} onClick={() => this.onClickMenu(Menus.View_1_1_All)}>1-1동 전체</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_1_2_1F)} onClick={() => this.onClickMenu(Menus.View_1_2_1F)}>1-2동 1층</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_1_2_2F)} onClick={() => this.onClickMenu(Menus.View_1_2_2F)}>1-2동 2층</div>
                    <div className={this.getMenuItemClassName(this.props.selectedMenu === Menus.View_All_Outside)} onClick={() => this.onClickMenu(Menus.View_All_Outside)}>솔브레인 외부 전체</div>
                    <div className={this.getMenuItemClassName(this.props.poi === Menus.CUP_WHITE)} onClick={() => this.onClickMenu(Menus.CUP_WHITE)}>하얀컵</div>
                    <div className={this.getMenuItemClassName(this.props.poi === Menus.CUP_BLUE)} onClick={() => this.onClickMenu(Menus.CUP_BLUE)}>파란컵</div>
                </div>
            </div>
        );
    }
}