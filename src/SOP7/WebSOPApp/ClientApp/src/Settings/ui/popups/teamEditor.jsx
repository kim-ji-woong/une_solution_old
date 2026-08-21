import React, { Component } from 'react';
import ConfirmDialog from '../../../Common/ui/confirmDialog';

import newStyles from '../../../Common/css/newStyle.module.css';
import newDefaults from '../../../Common/css/newDefault.module.css';
import settings from '../../css/settings.module.css';
import styles from '../../../Common/css/style.module.css';

import { SettingController } from '../../services/settingController';
import SettingResource from '../../resource/id';

class TeamEditor extends Component {
	constructor(props) {
		super(props);
		this.props = props;

		this.state = {
			confirmMessage: {
				visible: false,
				title: "",
				messages: [""],
				buttons: ["확인"],
				onClose: this.onCloseConfirmDialog,
				onClickButton: null
			},
		}

		this.refRegularTeamFile = React.createRef();
	}

	onClickUpload = (mode) => {
		if (mode === SettingResource.ID.excelMode.regularTeam) {
			this.refRegularTeamFile.current.click();
		}
	}

	onClickDownload = (mode) => {
		if (mode === SettingResource.ID.excelMode.regularTeam) {
			this.downloadRegularTeam();
		}
	}

	async downloadRegularTeam() {
		const [surcess, message] = await SettingController.requestDownloadRegularTeam();

		if (surcess === null) {
			this.showConfirmDialog("에러", [message], null, null);
		}
	}

	onSelectRegularTeamFile = (event) => {
		const file = event.target.files[0];
		this.refRegularTeamFile.current.value = "";

		this.props.settings.regularTeamFile = file;
	}

	showConfirmDialog = (title, messages, buttons, onClickButton) => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = true;
		confirmMessage.title = title;
		confirmMessage.buttons = buttons;
		confirmMessage.onClickButton = onClickButton;

		if (!messages) {
			confirmMessage.messages = [""];
		}
		else if (Array.isArray(messages)) {
			confirmMessage.messages = messages;
		}
		else {
			confirmMessage.messages = [messages];
		}

		this.setState({ confirmMessage });
	}

	onCloseConfirmDialog = () => {
		const confirmMessage = { ...this.state.confirmMessage };
		confirmMessage.visible = false;

		this.setState({ confirmMessage });
	}

	render() {
		return (
			<>
				<ul className={newStyles.stgTab + " " + newStyles.single}>
					<li><a className={newStyles.on + " " + styles.clickable}>일반</a></li>
				</ul>
				<div className={newStyles.stgList}>
				  <span className={newStyles.stgScroll}>
					<div className={newStyles.stgName}>
						<h5>조직정보 업데이트</h5>
						<span className={newStyles.stgTltp} data-tooltip="조직 정보를 엑셀파일 형식으로 업로드/다운로드 합니다."></span>
						<a onClick={() => this.onClickUpload(SettingResource.ID.excelMode.regularTeam)} className={newStyles.stgnRset + " " + newStyles.upload}>업로드</a>
						<a onClick={() => this.onClickDownload(SettingResource.ID.excelMode.regularTeam)} className={newStyles.stgnRset + " " + newDefaults.ml5}>다운로드</a>
						<input ref={this.refRegularTeamFile} className={settings.hidden} type='file' accept='.xls,.xlsx' onChange={this.onSelectRegularTeamFile} />
					</div>
			      </span>
				</div>
				{
					/* alert창 대신 사용 */
					this.state.confirmMessage.visible &&
					<ConfirmDialog title={this.state.confirmMessage.title} messages={this.state.confirmMessage.messages} buttons={this.state.confirmMessage.buttons} onClose={this.state.confirmMessage.onClose} onClickButton={this.state.confirmMessage.onClickButton} />
				}
			</>
		);
	}
}

export default TeamEditor;