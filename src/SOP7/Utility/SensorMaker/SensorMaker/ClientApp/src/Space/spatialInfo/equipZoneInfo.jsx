import React, { Component } from 'react';
import styles from '../css/spatial.module.css';
import { SpaceBody } from '../spaceBody';
import imgPlus from '../image/addSimple-01.png';
import imgMinus from '../image/minusSimple-01.png';
import imgPencil from '../image/pencilSimple-01.png';

export class EquipZoneInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            editMode: false,
            editText: "",
            expandNode: true
        }

        this.refEquipZoneName = React.createRef();
        this.refEquipZoneNameList = React.createRef();
        this.refEditEquipZoneName = React.createRef();

        // 사용자가 마우스로 조작하였는가?
        // true : 접혔다.
        // false : 펼쳐졌다.
        //this.manualEquipZoneNameExpand = null;
        //this.showEquipZoneNameResult = false;

        this.prevSelectedSensor = [null, null, null];
    }

    componentDidMount() {
        this.checkChildVisible();
    }

    componentDidUpdate(prevProps, prevState) {
        this.checkChildVisible();

        if (this.state.editMode && this.refEditEquipZoneName.current) {
            this.refEditEquipZoneName.current.focus();
        }
    }

    checkChildVisible() {
        this.checkChildVisibleData(this.refEquipZoneName.current, this.refEquipZoneNameList.current, this.state.expandNode);
    }

    checkChildVisibleData(mainElement, listElement, showChild) {
        if (mainElement) {
            if (showChild) {
                if (mainElement.dataset.show_child !== 'true') {
                    mainElement.dataset.show_child = 'true';
                }

                if (listElement) {
                    if (listElement.classList.contains(styles.on) === false) {
                        listElement.classList.add(styles.on);
                    }
                }
            }
            else if (listElement) {
                if (mainElement.dataset.show_child !== 'false') {
                    mainElement.dataset.show_child = 'false';
                }

                if (listElement.classList.contains(styles.on)) {
                    listElement.classList.remove(styles.on);
                }
            }
        }
    }

    showChild(e) {
    }

    isSelected() {
        return true;
    }

    onClickAdd(e, type) {
        const equipZone = this.props.equipZone;

        if (!equipZone) {
            return;
        }

        this.props.onAddItem(equipZone, type);
    }

    onClickRemove(e, type) {
        const equipZone = this.props.equipZone;
        const zone = this.props.zone;

        if (!equipZone || !zone) {
            return;
        }

        this.props.onRemoveItem([equipZone, zone], type);
    }

    onClickEdit(e, type) {
        const equipZone = this.props.equipZone;

        if (!equipZone) {
            return;
        }

        this.setState({ editMode: true, editText: equipZone.zoneName });
    }

    renameEquipZoneName() {
        if (this.props.equipZone && this.props.zone) {
            this.props.onRenameItem([this.props.equipZone, this.props.zone], this.state.editText.trim(), SpaceBody.Type_EquipZone);
        }

        this.setState({ editMode: false });
    }

    onKeyUp(e) {
        if (e.key === "Enter") {
            this.renameEquipZoneName();
        }
        else if (e.key === "Escape") {
            this.setState({ editMode: false });
        }
    }

    onChange(e) {
        const editText = e.target.value;
        this.setState({ editText });
    }

    onFocusout(e) {
        this.renameEquipZoneName();
    }

    getButtonImages() {
        if (!this.props.modeling && !this.props.dashboard) {
            return (
                <div className={styles.treeIconImageArea}>
                    <img className={styles.treeIconImage} src={imgMinus} alt="icon" onClick={(e) => this.onClickRemove(e, SpaceBody.Type_EquipZone)} />
                    <img className={styles.treeIconImage} src={imgPencil} alt="icon" onClick={(e) => this.onClickEdit(e, SpaceBody.Type_EquipZone)} />
                </div>
            );
        }

        return <></>;
    }

    render() {
        const equipZoneShowChild = this.state.expandNode;
        const equipZoneName = this.props.equipZone.displayText ? this.props.equipZone.displayText : this.props.equipZone.name;
        const equipZoneClassName = this.state.editMode ? styles.viewList3DepthSpen + " " + styles.hidden : styles.viewList3DepthSpen;

        return (
            <li>
                <div className={styles.viewListDepthParent}>
                    <div className={styles.viewList3DepthHead}>
                        <span ref={this.refEquipZoneName} className={equipZoneClassName} data-show_child={equipZoneShowChild} data-target_class='viewList3Depth' onClick={(e) => { this.showChild(e) }}>{equipZoneName}</span>
                    </div>
                    {
                        this.state.editMode &&
                        <input ref={this.refEditEquipZoneName} type="text" value={this.state.editText} onKeyUp={(e) => this.onKeyUp(e)} onChange={(e) => this.onChange(e)} onBlur={(e) => this.onFocusout(e)} />
                    }
                    {
                        this.getButtonImages()
                    }
                </div>
                {
                    this.props.sensorList &&
                    <ul ref={this.refEquipZoneNameList} className={equipZoneShowChild === 'true' ? styles.viewList4Depth + " " + styles.on : styles.viewList4Depth}>
                    </ul>
                }
            </li>
        );
    }
}