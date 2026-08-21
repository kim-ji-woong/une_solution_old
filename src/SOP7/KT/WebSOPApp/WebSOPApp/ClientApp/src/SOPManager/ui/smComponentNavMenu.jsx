import React, { Component } from 'react';
import SMMenuBtn from './smComponentMenuBtn';
import '../css/app.css';

class SMComponentNavMenu extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            menuButtons:
            [
                { isActive: true, dataType: "endpoint", menuName: "시작/끝" },
                { isActive: false, dataType: "process", menuName: "프로세스" },
                { isActive: false, dataType: "decision", menuName: "판단" },
                { isActive: false, dataType: "annotation", menuName: "설명" },
                { isActive: false, dataType: "internal", menuName: "내부 상황전파" },
                { isActive: false, dataType: "none", menuName: "없음" },
                { isActive: false, dataType: "delete", menuName: "삭제" }
            ],
        };

        const activeMenu = this.state.menuButtons.find( menu => menu.isActive === true);
        this.currentActiveDataType = activeMenu === null ? "" : activeMenu.dataType;
        this.props.onChange(this.currentActiveDataType);
    }

    onClickMenu = (dataType) =>
    {
        const buttons = [...this.state.menuButtons];
        const selectedMenu = buttons.find(menu => menu.dataType === dataType);

        if (selectedMenu === null)
        {
            if (this.currentActiveDataType.length > 0)
            {
                const currentActiveMenu = buttons.find(menu => menu.dataType === this.currentActiveDataType);

                if (currentActiveMenu !== null)
                {
                    currentActiveMenu.isActive = false;
                }

                this.setMenu("", buttons);
            }
        }
        else
        {
            selectedMenu.isActive = true;

            if (this.currentActiveDataType.length === 0)
            {
                this.setMenu(dataType, buttons);
            }
            else
            {
                if (this.currentActiveDataType !== selectedMenu.dataType)
                {
                    const currentActiveMenu = buttons.find(menu => menu.dataType === this.currentActiveDataType);

                    if (currentActiveMenu !== null)
                    {
                        currentActiveMenu.isActive = false;
                    }

                    this.setMenu(dataType, buttons);
                }
            }
        }
    }

    setMenu(dataType, buttons)
    {
        this.currentActiveDataType = dataType;
        this.props.onChange(this.currentActiveDataType);
        this.setState({menuButtons: buttons});
    }

    render() {
        return (
            <nav className="smComponentNavMenus">
                <ul>
                    {
                        this.state.menuButtons.map((menu) =>
                        (
                            <SMMenuBtn key={menu.dataType} menu={menu} onClickMenu={this.onClickMenu} />
                        ))
                    }
                </ul>
            </nav>
        );
    }
}

export default SMComponentNavMenu;