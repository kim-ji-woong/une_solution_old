import React, { Component } from 'react';
import '../../../Common/css/scroll.css';
import styles from '../../../Common/css/style.module.css';
import Receiver from '../../../Common/models/sections/receiver';
import SectionData from '../../../Common/models/sections/sectionData';
import SectionDataProcess from '../../../Common/models/sections/sectionDataProcess';
import SectionDataProcessMission from '../../../Common/models/sections/sectionDataProcessMission';
import Process from '../../../SOPSimulator/ui/missions/process';
import { TeamEditController } from '../../../TeamEditor/services/teamEditController';
import TreeNode from '../../../TeamEditor/ui/utility/treenode';
import TreeView from '../../../TeamEditor/ui/utility/treeview';
import bodyStyles from '../../css/body.module.css';
import '../../css/componentProperty.css';
import SopManagerResource from '../../resource/id';
import SopController from '../../services/sopController';
import SopDataManager from '../../services/sopDataManager';


//import axios from 'axios';

//axios.get('https://my-json-server.typicode.com/zofqofhtltm8015/fs/user').then((Response) => {
//	console.log(Response.data);
//}).catch((Error) => {
//	console.log(Error);
//})


class ProcessProperty extends Component {
	static cssStyles = styles;

	static Integer_Type = 20;
	static Float_Type = 21;
	static Double_Type = 22;
	static String_Type = 23;
	static Long_Type = 24;
	static Boolean_Type = 25;
	static Short_Type = 26;
	static Byte_Type = 27;

	static MissionNormalType = 0;
	static MissionExternalType = 1;

	constructor(props) {
		super(props);
		this.props = props;

		const sectionData = new SectionDataProcess();

		if (this.props.sectionData) {
			SectionDataProcess.copyTo(this.props.sectionData, sectionData);
		}

		this.state = {
			instance: this,
			sectionData: sectionData,
			missions: sectionData ? ProcessProperty.copyMissions(sectionData.missions) : [],
			teamType: ProcessProperty.getDefaultTeamType(this.props.sectionData),
			teamTreeData: null,
			teamAllTreeDatas: {...this.props.teamAllTreeDatas},
			receiverName: sectionData.receiverName ? sectionData.receiverName : "",
			selectedTeam: null,
			receiversOn: false,
			missionsOn: false,
			includeChildTeams: false,
			/*autoRun: false,*/
			externalProgram: {
				datas: [],
				selectedDataIndex: -1,
				showList: false
            },
			prevProps: this.props
		}

		this.refReceivers = React.createRef();
		this.refMissions = React.createRef();
		this.refCheckIncludeChildTeams = React.createRef();
		this.refTitle = React.createRef();
		this.refMissionBody = React.createRef();
		this.refExternalPrograms = React.createRef();

		this.getExternalPrograms(false);
	}

	componentWillUnmount() {
		// 창이 닫히게 될 경우 편집한 내용을 저장한다.
		this.saveSectionData(false);
    }

	static getDerivedStateFromProps(props, state) {
		if (props === state.prevProps) {
			return state;
		}

		if (props.sectionData !== state.prevProps.sectionData && state.sectionData) {
			// 다른 Process를 선택하여 창이 바뀌게 될 경우 편집한 내용을 저장한다.
			state.instance.saveSectionData(false);
        }

		const sectionData = new SectionDataProcess();

		if (props.sectionData) {
			SectionDataProcess.copyTo(props.sectionData, sectionData);
		}

		state.instance.refTitle.current.value = state.instance.refTitle.current.text = sectionData.text;

		return {
			instance: state.instance,
			sectionData: sectionData,
			missions: sectionData ? ProcessProperty.copyMissions(sectionData.missions) : [],
			teamType: ProcessProperty.getDefaultTeamType(props.sectionData),
			teamTreeData: null,
			teamAllTreeDatas: { ...props.teamAllTreeDatas },
			receiverName: sectionData.receiverName ? sectionData.receiverName : "",
			selectedTeam: null,
			receiversOn: state.receiversOn,
			missionsOn: state.missionsOn,
			includeChildTeams: state.includeChildTeams,
			/*autoRun: state.autoRun,*/
			externalProgram: state.externalProgram,
			prevProps: props
		};
	}

	static getDefaultTeamType(sectionData) {
		if (sectionData?.receivers) {
			const receiverCount = sectionData.receivers.length;
			let temporaryNormal = false, temporaryEmergency = false;

			for (let i = 0; i < receiverCount; i++) {
				const receiver = sectionData.receivers[i];

				if (receiver.teamType === Receiver.RegularTeam) {
					return Receiver.RegularTeam;
				}
				else if (receiver.teamType === Receiver.TemporaryNormalTeam) {
					temporaryNormal = true;
				}
				else if (receiver.teamType === Receiver.TemporaryEmergencyTeam) {
					temporaryEmergency = true;
				}
			}

			if (temporaryNormal) {
				return Receiver.TemporaryNormalTeam;
			}
			else if (temporaryEmergency) {
				return Receiver.TemporaryEmergencyTeam;
			}
		}

		return Receiver.RegularTeam;
	}

	static copyMissions(missions) {
		if (!missions) {
			return [];
		}

		const copied = [];
		const missionCount = missions.length;

		for (let i = 0; i < missionCount; i++) {
			const mission = { ...missions[i] };
			copied.push(mission);
		}

		return copied;
    }

	/*getNext(element) {
		const count = element.parentNode.children.length;
		let target = -1;

		for (let i = 0; i < count; i++) {
			if (i === target) {
				return element.parentNode.children[i];
			}

			if (element.parentNode.children[i] === element) {
				target = i + 1;
			}
		}

		return null;
	}*/

	onClickCascade = (event) => {
		this.slideCascade(event.target);
	}

	async slideCascade(element) {
		//const next = this.getNext(element);
		let teamTreeDatas = null;
		let receiversOn = this.state.receiversOn;
		let missionsOn = this.state.missionsOn;

		const teamAllTreeDatas = { ...this.state.teamAllTreeDatas };

		if (element === this.refReceivers.current) {
			if (this.state.teamType === Receiver.RegularTeam) {
				teamTreeDatas = await TeamEditController.DisplayRegular();
				SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.RegularTeam);
				teamAllTreeDatas.regular = teamTreeDatas;
			}
			else if (this.state.teamType === Receiver.TemporaryNormalTeam) {
				teamTreeDatas = await TeamEditController.DisplayTemporary(true);
				SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.TemporaryNormalTeam);
				teamAllTreeDatas.normal = teamTreeDatas;
			}
			else if (this.state.teamType === Receiver.TemporaryEmergencyTeam) {
				teamTreeDatas = await TeamEditController.DisplayTemporary(false);
				SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.TemporaryEmergencyTeam);
				teamAllTreeDatas.emergency = teamTreeDatas;
			}

			if (element.classList.contains(ProcessProperty.cssStyles.on)) {
				receiversOn = false;
			}
			else {
				receiversOn = true;
				missionsOn = false;
			}
		}
		else {
			if (element.classList.contains(ProcessProperty.cssStyles.on)) {
				missionsOn = false;
			}
			else {
				receiversOn = false;
				missionsOn = true;
			}
        }

		/*if (element.classList.contains(ProcessProperty.cssStyles.on)) {
			element.classList.remove(ProcessProperty.cssStyles.on);
			if (next.classList.contains(ProcessProperty.cssStyles.on)) {
				next.classList.remove(ProcessProperty.cssStyles.on);
			}
		}
		else {
			element.classList.add(ProcessProperty.cssStyles.on);
			if (next.classList.contains(ProcessProperty.cssStyles.on) === false) {
				next.classList.add(ProcessProperty.cssStyles.on);
			}
		}*/

		this.setState({ teamTreeData: teamTreeDatas, teamAllTreeDatas, receiversOn, missionsOn });
	}

	getChildTextAreaValue(element, missionItems) {
		if (element.tagName === "TEXTAREA") {
			missionItems.push(element.value);
			return;
		}

		for (let i = 0; i < element.children.length; i++) {
			this.getChildTextAreaValue(element.children[i], missionItems);
		}
    }

	getMissionItems() {
		const missionItems = [];

		for (let i = 0; i < this.refMissionBody.current.children.length; i++) {
			this.getChildTextAreaValue(this.refMissionBody.current.children[i], missionItems);
		}

		return missionItems;
	}

	onClickApply(ok) {
		if (ok) {
			this.saveSectionData(true);
		}
		else {
			this.setState({
				sectionData: null,
				missions: [],
				teamType: Receiver.None,
				teamTreeData: null,
				selectedTeam: null,
				receiversOn: false,
				missionsOn: false,
				includeChildTeams: false/*,
				autoRun: false*/
			});
        }
	}

	saveSectionData(shouldUpdate) {
		const sectionData = { ...this.state.sectionData };
		sectionData.receiverName = this.state.receiverName;
		sectionData.text = this.refTitle.current.value;
		//sectionData.autoRun = this.state.autoRun;

		sectionData.missions = [];
		const missions = [...this.state.missions];

		const missionTexts = this.getMissionItems();
		const missionTextCount = missionTexts.length;

		let nIndex = 0;
		const missionCount = missions.length;

		for (let i = 0; i < missionCount; i++) {
			const mission = missions[i];

			if (mission.missionType === ProcessProperty.MissionNormalType) {
				if (nIndex < missionTextCount) {
					const missionText = missionTexts[nIndex++];
					mission.missionText = missionText;
				}
			}
			else if (mission.missionType === ProcessProperty.MissionExternalType) {
				mission.OrderIndex = i + 1;
            }

			sectionData.missions.push(mission);
        }

		this.props.onApplyComponentProperty(sectionData, this.props.actionStep, shouldUpdate);
    }

	onChangeTeamMode(teamType) {
		this.changeTeamMode(teamType);
	}

	async changeTeamMode(teamType) {
		let teamTreeDatas = null;
		const teamAllTreeDatas = { ...this.state.teamAllTreeDatas };

		if (teamType === Receiver.RegularTeam) {
			teamTreeDatas = await TeamEditController.DisplayRegular();
			SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.RegularTeam);
			teamAllTreeDatas.regular = teamTreeDatas;
		}
		else if (teamType === Receiver.TemporaryNormalTeam) {
			teamTreeDatas = await TeamEditController.DisplayTemporary(true);
			SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.TemporaryNormalTeam);
			teamAllTreeDatas.normal = teamTreeDatas;
		}
		else if (teamType === Receiver.TemporaryEmergencyTeam) {
			teamTreeDatas = await TeamEditController.DisplayTemporary(false);
			SopDataManager.setTeamTreeDataChecked(teamTreeDatas, this.state.sectionData?.receivers, Receiver.TemporaryEmergencyTeam);
			teamAllTreeDatas.emergency = teamTreeDatas;
		}

		this.setState({ teamTreeData: teamTreeDatas, teamAllTreeDatas: teamAllTreeDatas, teamType: teamType });
    }

	onTreeNodeChanged = (team, event) => {
		if (event === undefined) {
			if (this.state.selectedTeam !== team) {
				this.setState({ selectedTeam: team });
			}
		}
		else if (event.type === TreeView.EventCheckedChanged) {
			this.onTreeNodeCheckedChanged(team, this.state.teamType);
        }
	}

	onTreeNodeCheckedChanged(team, teamType) {
		if (team) {
			let receivers = this.state.sectionData.receivers;

			if (!receivers) {
				this.state.sectionData.receivers = [];
				receivers = this.state.sectionData.receivers;
            }

			if (receivers) {
				if (team.checked === TreeNode.CHECKED_NONE) {
					this.removeReceiver(receivers, team.ID, teamType);
				}
				else if (team.checked === TreeNode.CHECKED_ALL) {
					this.addReceiver(receivers, team.ID, teamType);
				}

				const receiverName = SopDataManager.getReceiverText(this.state.sectionData.receivers, this.state.teamAllTreeDatas);

				if (receiverName !== this.state.receiverName) {
					this.setState({ receiverName });
                }
            }
        }
	}

	removeReceiver(receivers, teamID, teamType) {
		const receiverCount = receivers.length;

		for (let i = 0; i < receiverCount; i++) {
			const receiver = receivers[i];

			if (receiver.teamType === teamType && receiver.teamID == teamID) {
				receivers.splice(i, 1);
				break;
            }
        }
	}

	addReceiver(receivers, teamID, teamType) {
		const receiverCount = receivers.length;

		for (let i = 0; i < receiverCount; i++) {
			const receiver = receivers[i];

			if (receiver.teamType === teamType && receiver.teamID == teamID) {
				// 이미 존재한다.
				return;
			}
		}

		const receiver = { teamType, teamID };
		receivers.push(receiver);
    }

	onMissionTextChange(event, mission) {
		mission.missionText = event.target.value;
		this.setState({ missions: this.state.missions });
	}

	onCheckIncludeChildTeam = (event) => {
		this.setState({ includeChildTeams: event.target.checked });
	}

	onCheckAutoRun = (event) => {
		if (this.state.sectionData) {
			const sectionData = { ...this.state.sectionData };
			sectionData.autoRun = event.target.checked;
			this.setState({ sectionData });
        }
		//this.setState({ autoRun: event.target.checked });
	}

	onClickAddMission = (event) => {
		if (!this.state.missions || !this.state.sectionData) {
			return;
		}

		const missions = [...this.state.missions];
		const mission = new SectionDataProcessMission(-1, "");
		mission.missionType = ProcessProperty.MissionNormalType;
		missions.push(mission);

		this.setState({ missions });
	}

	onClickAddExternalProgram(add) {
		const externalProgram = { ...this.state.externalProgram };

		if (add) {
			const selectedIndex = externalProgram.selectedDataIndex;
			const programDatas = externalProgram.datas;

			if (selectedIndex < 0 || selectedIndex >= programDatas.length) {
				alert(SopManagerResource.ID.messages.selectAddProgram);
				return;
			}

			const programData = programDatas[selectedIndex];
			const parameterCount = programData.parameters.length;
			const arrDatas = [];

			for (let i = 1; i < parameterCount; i++) {
				const parameter = programData.parameters[i];
				const [success, data] = this.checkParameter(selectedIndex, i - 1, parameter.valueType, parameter.isNullable);

				if (success === false) {
					alert(SopManagerResource.format(SopManagerResource.ID.messageFormat.checkNthParameters, i));
					return;
				}
				else {
					arrDatas.push(data);
				}
			}

			// arrDatas 사용할 것
			this.addExternalMission(programData.program, arrDatas);
		}
		else {
			externalProgram.showList = false;
			this.setState({ externalProgram });
        }
	}

	addExternalMission(program, parameters) {
		const missions = [...this.state.missions];

		const externalMission = {
			missionType: ProcessProperty.MissionExternalType,
			programID: program.id,
			processID: this.state.sectionData.id,
			programName: program.description && program.description.length > 0 ? program.description : program.exeName,
			parameters: [null]
		};

		const parameterCount = parameters.length;

		for (let i = 0; i < parameterCount; i++) {
			externalMission.parameters.push(parameters[i].toString());
		}

		const externalProgram = { ...this.state.externalProgram };
		externalProgram.selectedDataIndex = -1;
		
		missions.push(externalMission);
		this.clearExternalProgramParameters(externalProgram);
		this.setState({missions, externalProgram});
	}

	clearExternalProgramParameters(externalProgram) {
		const programDatas = externalProgram.datas;
		const programDataCount = programDatas.length;

		for (let i = 0; i < programDataCount; i++) {
			this.clearExternalProgramParameter(i);
        }
	}

	clearExternalProgramParameter(programIndex) {
		if (!this.refExternalPrograms.current) {
			return;
		}

		const count = this.refExternalPrograms.current.children.length;

		if (programIndex >= count) {
			return;
		}

		const tr = this.refExternalPrograms.current.children[programIndex];

		if (tr.children.length < 2) {
			return;
		}

		const td = tr.children[1];
		const tdChildCount = td.children.length;

		if (tdChildCount === 0)
			return;

		const div = td.children[0];
		const childCount = div.children.length;

		for (let i = 0; i < childCount; i++) {
			const child = div.children[i];

			if (child.tagName === "DIV") {
				const divChildCount = child.children.length;
				
				for (let j = 0; j < divChildCount; j++) {
					const divChild = child.children[j];

					if (divChild.tagName === "P") {
						if (divChild.innerText.includes("전달인자")) {
							for (let k = 0; k < divChild.children.length; k++) {
								const element = divChild.children[k];

								if (element.tagName === "INPUT") {
									element.value = "";
								}
							}
						}
					}
				}
			}
		}
    }

	checkParameter(programIndex, parameterIndex, valueType, isNullable) {
		if (!this.refExternalPrograms.current) {
			return [false, null];
		}

		const count = this.refExternalPrograms.current.children.length;

		if (programIndex >= count) {
			return [false, null];
        }

		const tr = this.refExternalPrograms.current.children[programIndex];

		if (tr.children.length < 2) {
			return [false, null];
		}

		const td = tr.children[1];
		const tdChildCount = td.children.length;

		if (tdChildCount === 0)
			return [false, null];

		const div = td.children[0];
		const childCount = div.children.length;

		let paramCount = 0;
		let value = null;

		for (let i = 0; i < childCount; i++) {
			const child = div.children[i];

			if (child.tagName === "DIV") {
				const divChildCount = child.children.length;
				let find = false;

				for (let j = 0; j < divChildCount; j++) {
					const divChild = child.children[j];

					if (divChild.tagName === "P") {
						if (divChild.innerText.includes("전달인자")) {
							if (paramCount++ === parameterIndex) {
								for (let k = 0; k < divChild.children.length; k++) {
									const element = divChild.children[k];

									if (element.tagName === "INPUT") {
										value = element.value;
										find = true;
										break;
									}
								}

								break;
							}
						}
					}
				}

				if (find) {
					break;
                }
			}
		}

		if (valueType === ProcessProperty.Integer_Type ||
			valueType === ProcessProperty.Short_Type ||
			valueType === ProcessProperty.Long_Type ||
			valueType === ProcessProperty.Byte_Type) {
			return this.getIntegerValue(value, isNullable);
		}
		else if (valueType === ProcessProperty.Float_Type ||
			valueType === ProcessProperty.Double_Type) {
			return this.getFloatValue(value, isNullable);
		}
		else if (valueType === ProcessProperty.Boolean_Type) {
			return this.getBooleanValue(value, isNullable);
		}
		else if (valueType === ProcessProperty.String_Type) {
			return this.getStringValue(value, isNullable);
		}

		return [false, null];
	}

	getIntegerValue(value, isNullable) {
		if (value === null || value.length === 0) {
			if (isNullable) {
				return [true, null];
			}
			else {
				return [false, null];
			}
		}

		const data = parseInt(value.trim());

		if (data) {
			return [true, data];
        }

		return [false, null];
	}

	getFloatValue(value, isNullable) {
		if (value === null || value.length === 0) {
			if (isNullable) {
				return [true, null];
			}
			else {
				return [false, null];
			}
		}

		const data = parseFloat(value.trim());

		if (data) {
			return [true, data];
		}

		return [false, null];
	}

	getBooleanValue(value, isNullable) {
		if (value === null || value.length === 0) {
			if (isNullable) {
				return [true, null];
			}
			else {
				return [false, null];
			}
		}

		const lower = value.trim().toLowerCase();

		if (lower === "true" || lower === "1" || lower === "참") {
			return [true, true];
		}
		else if (lower === "false" || lower === "0" || lower === "거짓") {
			return [true, false];
		}

		return [false, null];
	}

	getStringValue(value, isNullable) {
		if (value === null || value.length === 0) {
			if (isNullable) {
				return [true, ""];
			}
			else {
				return [false, null];
			}
		}

		return [true, value.trim()];
	}

	onClickExternalPrograms = (event) => {
		const externalProgram = { ...this.state.externalProgram };
		
		if (externalProgram.showList) {
			externalProgram.showList = false;
			this.setState({ externalProgram });
		}
		else {
			this.getExternalPrograms(true);
		}
	}

	async getExternalPrograms(showTable) {
		const [programDatas, message] = await SopController.requestExternalPrograms();

		if (programDatas === null) {
			alert(message);
		}
		else {
			const externalProgram = {
				datas: programDatas.programs,
				selectedDataIndex: -1,
				showList: showTable
			};

			this.setState({ externalProgram });
        }
	}

	getReceiversClassName() {
		if (this.state.receiversOn) {
			return ProcessProperty.cssStyles.on;
		}

		return "";
	}

	getMissionsClassName() {
		if (this.state.missionsOn) {
			return ProcessProperty.cssStyles.on;
		}

		return "";
	}

	onMissionCheckedChanged(event, mission) {
		mission.checked = event.target.checked;
		this.setState({ missions: this.state.missions });
    }

	getMissionContents(mission, index) {
		const checked = mission.checked ? true : false;

		if (mission.missionType === ProcessProperty.MissionNormalType) {
			if (index % 2 === 0) {
				return (
					<tr key={"missoin_" + index}>
						<td><input type="checkbox" checked={checked} onChange={(event) => this.onMissionCheckedChanged(event, mission)} /></td>
						<td className={ProcessProperty.cssStyles.tal}>
							<div className={"scroll-wrapper " + ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer scroll-textarea"} id="pos_relative">
								<div className="scroll-content" id="mission_odd">
									<textarea name="" id="" cols="30" rows="10" className={ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer"} value={mission.missionText} onChange={(event) => this.onMissionTextChange(event, mission)}></textarea>
								</div>
								<div className="scroll-element scroll-x">
									<div className="scroll-element_outer">
										<div className="scroll-element_size"></div>
										<div className="scroll-element_track"></div>
										<div className="scroll-bar" id="width_100Pro"></div>
									</div>
								</div>
								<div className="scroll-element scroll-y">
									<div className="scroll-element_outer">
										<div className="scroll-element_size"></div>
										<div className="scroll-element_track"></div>
										<div className="scroll-bar" id="height_100Pro"></div>
									</div>
								</div>
							</div>
						</td>
					</tr>
				);
			}

			return (
				<tr key={"missoin_" + index} className={ProcessProperty.cssStyles.blu}>
					<td><input type="checkbox" checked={checked} onChange={(event) => this.onMissionCheckedChanged(event, mission)} /></td>
					<td className={ProcessProperty.cssStyles.tal}>
						<div className={"scroll-wrapper " + ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer scroll-textarea"} id="pos_relative">
							<div className="scroll-content" id="mission_even">
								<textarea name="" id="" cols="30" rows="10" className={ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer"} value={mission.missionText} onChange={(event) => this.onMissionTextChange(event, mission)}></textarea>
							</div>
							<div className="scroll-element scroll-x">
								<div className="scroll-element_outer">
									<div className="scroll-element_size"></div>
									<div className="scroll-element_track"></div>
									<div className="scroll-bar" id="width_100Pro"></div>
								</div>
							</div>
							<div className="scroll-element scroll-y">
								<div className="scroll-element_outer">
									<div className="scroll-element_size"></div>
									<div className="scroll-element_track"></div>
									<div className="scroll-bar" id="height_100Pro"></div>
								</div>
							</div>
						</div>
					</td>
				</tr>
			);
		}
		else if (mission.missionType === ProcessProperty.MissionExternalType) {
			const strParam = this.getParameterString(mission.parameters);
			const className = index % 2 === 1 ? ProcessProperty.cssStyles.blu : "";

			return (
				<tr key={"missoin_" + index} className={className}>
					<td><input type="checkbox" checked={checked} onChange={(event) => this.onMissionCheckedChanged(event, mission)} /></td>
					<td className={ProcessProperty.cssStyles.tal}>
						<div className={ProcessProperty.cssStyles.sprmUmoo}>
							<span>외부</span>
							<p>{mission.programName}</p>
							{
								strParam.length > 0 &&
								<br />
							}
							{
								strParam.length > 0 &&
								<p>{strParam}</p>
							}
						</div>
					</td>
				</tr>
				);
		}

		return <tr key={"missoin_" + index}></tr>
	}

	getParameterString(parameters) {
		const paramCount = parameters.length;
		let strParam = "";

		for (let i = 1; i < paramCount; i++) {
			// 첫번째 parameter는 무시한다.
			if (strParam.length === 0) {
				strParam = parameters[i];
			}
			else {
				strParam += ", " + parameters[i];
            }
		}

		if (strParam.length > 0) {
			strParam = "(" + strParam + ")";
		}

		return strParam;
    }

	missionContents() {
		const missions = [...this.state.missions];

		if (missions) {
			return (
				<>
					{
						missions.map((mission, index) => (
							this.getMissionContents(mission, index)
					))
				}
				</>
			);
		}

		return <></>;
	}

	onChangeExternalProgram(index) {
		const externalProgram = { ...this.state.externalProgram };
		externalProgram.selectedDataIndex = index;
		this.setState({ externalProgram });
	}

	getDataType(type) {
		if (type === ProcessProperty.Integer_Type) {
			return "(" + SopManagerResource.ID.dataType.integer + ")";
		}
		else if (type === ProcessProperty.Float_Type) {
			return "(" + SopManagerResource.ID.dataType.float + ")";
		}
		else if (type === ProcessProperty.Double_Type) {
			return "(" + SopManagerResource.ID.dataType.double + ")";
		}
		else if (type === ProcessProperty.String_Type) {
			return "(" + SopManagerResource.ID.dataType.string + ")";
		}
		else if (type === ProcessProperty.Long_Type) {
			return "(" + SopManagerResource.ID.dataType.long + ")";
		}
		else if (type === ProcessProperty.Boolean_Type) {
			return "(" + SopManagerResource.ID.dataType.boolean + ")";
		}
		else if (type === ProcessProperty.Short_Type) {
			return "(" + SopManagerResource.ID.dataType.short + ")";
		}
		else if (type === ProcessProperty.Byte_Type) {
			return "(" + SopManagerResource.ID.dataType.byte + ")";
		}

		return "";
    }

	makeExternalProgramParameter(parameter, index) {
		const key = "externalParameter" + parameter.parameterIndex + "_" + index;

		// 첫번째 요소는 사용하지 않는다.
		if (parameter.parameterIndex === 0) {
			return <div key={key}></div>
		}

		if (parameter.isNullable) {
			return (
				<div key={key}>
					<p>{`전달인자${parameter.parameterIndex}`}< input type="text" className={ProcessProperty.cssStyles.sprmTxt} placeholder={parameter.parameterName + this.getDataType(parameter.valueType)} /><br /></p>
				</div>
			);
		}

		return (
			<div key={key}>
				<p>{`전달인자${parameter.parameterIndex}`}< input type="text" className={ProcessProperty.cssStyles.sprmTxt} placeholder={parameter.parameterName + this.getDataType(parameter.valueType)} /><br /></p>
			</div>
		);
    }

	makeExternalProgramContents(programData, index) {
		return (
			<tr key={"externalProgram_" + index} className = { ProcessProperty.cssStyles.blu } >
				<td><input type="radio" name="sprmRdo2" checked={this.state.externalProgram.selectedDataIndex === index} onChange={() => this.onChangeExternalProgram(index)} /></td>
				<td className={ProcessProperty.cssStyles.tal}>
					<div className={ProcessProperty.cssStyles.sprmUmoo}>
						<p>{programData.program.description}</ p><br />
						<p>{`이름 : ${programData.program.exeName}`}</ p>
						{
							programData.parameters.map((parameter, paramIndex) => this.makeExternalProgramParameter(parameter, paramIndex))
						}
					</div>
				</td>
			</tr>
		);
	}


	getExternalProgramContents() {
		const externalProgram = { ...this.state.externalProgram };
		const programCount = externalProgram.datas.length;

	   if (externalProgram.showList && programCount === 0) {
			return (
				<div className={ProcessProperty.cssStyles.sprmDsc}>
					<p>사용할 수 있는 외부 프로그램이 존재하지 않습니다.</p>
				</div>
			);
	   }

		if (externalProgram.showList === false || programCount === 0) {
			return <></>
		}

		return (
			<table className={ProcessProperty.cssStyles.sprmTb}>
				<colgroup>
					<col className="width_15Pro"></ col>
					<col className="width_85Pro"></ col>
				</colgroup>
				<tbody ref={this.refExternalPrograms}>
					{
						externalProgram.datas.map((programData, index) => this.makeExternalProgramContents(programData, index))
					}
				</tbody>
			</table>
		);
	}

	onClickUp = () => {
		const missions = [...this.state.missions];
		let firstIndex = -1;

		const missionCount = missions.length;

		for (let i = 0; i < missionCount; i++) {
			const mission = missions[i];

			if (mission.checked) {
				if (firstIndex < 0) {
					firstIndex = i;
					break;
                }
            }
		}

		if (firstIndex <= 0) {
			// 더 위로 이동할 수 없다.
			return;
		}

		for (let i = 1; i < missionCount; i++) {
			const mission = missions[i];

			if (mission.checked) {
				const temp = missions[i - 1];
				missions[i - 1] = missions[i];
				missions[i] = temp;
			}
		}

		this.setState({ missions });
	}

	onClickDown = () => {
		const missions = [...this.state.missions];
		let lastIndex = -1;

		const missionCount = missions.length;

		for (let i = missionCount-1; i >= 0; i--) {
			const mission = missions[i];

			if (mission.checked) {
				if (lastIndex < 0) {
					lastIndex = i;
					break;
				}
			}
		}

		if (lastIndex < 0 || lastIndex === missionCount - 1) {
			// 더 아래로 이동할 수 없다.
			return;
		}

		for (let i = missionCount-2; i >= 0; i--) {
			const mission = missions[i];

			if (mission.checked) {
				const temp = missions[i + 1];
				missions[i + 1] = missions[i];
				missions[i] = temp;
			}
		}

		this.setState({ missions });
	}

	onClickDel = () => {
		const missions = [ ...this.state.missions ];
		const missionCount = missions.length;
		let changed = false;

		for (let i = missionCount - 1; i >= 0; i--) {
			const mission = missions[i];

			if (mission.checked) {
				missions.splice(i, 1);
				changed = true;
            }
		}

		if (changed) {
			this.setState({ missions });
        }
	}

	//onClickExternalPrograms = () => {
	//	programDatas = [
	//		{
	//			description: '밸브 잠금',
	//			exeName: 'text.exe',
	//			id: '1',
	//			installPath: 'null'
	//		}
	//	]

	//	return programDatas;
	//}


	//const programDatas = (props) => {
	//	const [programDatas,map, setData] = useState(['1', '2', '3']);


	render() {
		const autoRun = this.state.sectionData?.autoRun ? true : false;

			return (
				<div className={ProcessProperty.cssStyles.sprCont + " " + bodyStyles.noDrag}>
					<div className={ProcessProperty.cssStyles.sprTop}>
						<div className={ProcessProperty.cssStyles.sprtTitle}>
							<h4>프로세스</h4>
							<p>
								<label className={ProcessProperty.cssStyles.clickable}>
									<input type="checkbox" name="smsChk" id="smsChk" className={bodyStyles.labelInput} checked={autoRun} onChange={this.onCheckAutoRun} />
								자동실행
							</label>
							</p>
						</div>
						<dl className={ProcessProperty.cssStyles.sprtIpt}>
							<dt>제목</dt>
							<dd><input ref={this.refTitle} type="text" defaultValue={this.state.sectionData?.text} onChange={() => {}} /></dd>
						</dl>
						<dl className={ProcessProperty.cssStyles.sprtIpt}>
							<dt>수신자</dt>
							<dd>
								<div id="position_relative" className={"scroll-wrapper " + ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer scroll-textarea"}>
									<div id="process_scrollContent" className="scroll-content">
										<textarea name="" id="" cols="30" rows="10" className={ProcessProperty.cssStyles.sprtTxt + " scrollbar-outer"} value={this.state.receiverName} onChange={() => {}}>
										</textarea>
									</div>
									<div className="scroll-element scroll-x">
										<div className="scroll-element_outer">
											<div className="scroll-element_size">
											</div>
											<div className="scroll-element_track">
											</div>
											<div className="scroll-bar" id="width_100px">
											</div>
										</div>
									</div>
									<div className="scroll-element scroll-y">
										<div className="scroll-element_outer">
											<div className="scroll-element_size">
											</div>
											<div className="scroll-element_track">
											</div>
											<div className="scroll-bar" id="height_100px">
											</div>
										</div>
									</div>
								</div>
							</dd>
						</dl>
					</div>
					<div className={ProcessProperty.cssStyles.sprMid}>
						<div className="scroll-wrapper scrollbar-outer">
							<div className="scrollbar-outer scroll-content scroll-scrolly_visible">
								<div className={ProcessProperty.cssStyles.sprmCont}>
									<dl className={ProcessProperty.cssStyles.sprmAcdn}>
										<dt ref={this.refReceivers} className={this.getReceiversClassName()} onClick={this.onClickCascade}>수신자</dt>
										<dd className={this.getReceiversClassName()}>
											<div className={ProcessProperty.cssStyles.sprmTeam}>
												<ul className={ProcessProperty.cssStyles.sprmRdo}>
													<li>
														<label className={ProcessProperty.cssStyles.clickable}>
															<input type="radio" name="sprmRdo" className={bodyStyles.labelInput} id={ProcessProperty.cssStyles.sprmRdo01} checked={this.state.teamType === Receiver.RegularTeam} onChange={() => this.onChangeTeamMode(Receiver.RegularTeam)} />
														정규조직
													</label>
													</li>
													<li>
														<label className={ProcessProperty.cssStyles.clickable}>
															<input type="radio" name="sprmRdo" className={bodyStyles.labelInput} id={ProcessProperty.cssStyles.sprmRdo02} checked={this.state.teamType === Receiver.TemporaryNormalTeam} onChange={() => this.onChangeTeamMode(Receiver.TemporaryNormalTeam)} />
														평일 비상조직
													</label>
													</li>
													<li>
														<label className={ProcessProperty.cssStyles.clickable}>
															<input type="radio" name="sprmRdo" className={bodyStyles.labelInput} id={ProcessProperty.cssStyles.sprmRdo03} checked={this.state.teamType === Receiver.TemporaryEmergencyTeam} onChange={() => this.onChangeTeamMode(Receiver.TemporaryEmergencyTeam)} />
														야간/휴일 비상조직
													</label>
													</li>
												</ul>
												<TreeView treeViewID="sopPropertyTree" treeViewHeight={250} teamTreeData={this.state.teamTreeData} onTreeNodeChanged={this.onTreeNodeChanged} useCheckBox={TreeNode.CheckBox_NormalUse} style={{"height": "250px"}} />
											</div>
										</dd>
										<dt ref={this.refMissions} className={this.getMissionsClassName()} onClick={this.onClickCascade}>임무내용</dt>
										<dd className={this.getMissionsClassName()}>
											<div className={ProcessProperty.cssStyles.sprmUdn}>
												<a className={ProcessProperty.cssStyles.up} onClick={this.onClickUp}>up</a>
												<a className={ProcessProperty.cssStyles.down} onClick={this.onClickDown}>down</a>
												<a className={ProcessProperty.cssStyles.del} onClick={this.onClickDel}>선택삭제</a>
											</div>
											<button className={ProcessProperty.cssStyles.sprmAdd} onClick={this.onClickAddMission}>+ 임무내용 추가</button>
											<table className={ProcessProperty.cssStyles.sprmTb}>
												<colgroup>
													<col className="width_15Pro" />
													<col className="width_85Pro" />
												</colgroup>
												<tbody ref={this.refMissionBody}>
													{
														this.missionContents()
													}
												</tbody>
											</table>
											<div className={ProcessProperty.cssStyles.sprmDsc}>
												<p>외부로 임무 내용이 전파될 수 있으므로, 개인정보 보호를 위해서 특정 개인의 정보는 입력하지 말아 주십시오.</p>
											</div>
											<a className={ProcessProperty.cssStyles.sprmIport} onClick={this.onClickExternalPrograms}>외부 프로그램 불러오기</a>
											{
												this.getExternalProgramContents()
											}
											{
												this.state.externalProgram.showList && this.state.externalProgram.datas.length > 0 && (
													<ul className={ProcessProperty.cssStyles.sprmBtn}>
														<li><a className={ProcessProperty.cssStyles.clickable} onClick={() => this.onClickAddExternalProgram(true)}>추가</a></li>
														<li><a className={ProcessProperty.cssStyles.clickable} onClick={() => this.onClickAddExternalProgram(false)}>취소</a></li>
													</ul>
												)
											}
										</dd>
									</dl>
								</div>{/*sprmCont*/}
								<div className="scroll-element scroll-x">
									<div className="scroll-element_outer">
										<div className="scroll-element_size"></div>
										<div className="scroll-element_track"></div>
										<div className="scroll-bar3"></div>
									</div>
								</div>
								<div className="scroll-element scroll-y">
									<div className="scroll-element_outer">
										<div className="scroll-element_size"></div>
										<div className="scroll-element_track"></div>
										<div className="scroll-bar4"></div>
									</div>
								</div>
							</div>
						</div>
					</div>{/*sprMid*/}
					<div className={ProcessProperty.cssStyles.sprBot}>
						<a className={ProcessProperty.cssStyles.clickable} onClick={() => this.onClickApply(true)}>확인</a>
						<a className={ProcessProperty.cssStyles.clickable} onClick={() => this.onClickApply(false)}>취소</a>
					</div>{/*sprBot*/}
				</div>
			);
		}
}

export default ProcessProperty;