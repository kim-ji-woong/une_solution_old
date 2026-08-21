import React, { Component } from 'react';
//import DeniReactTreeView from "deni-react-treeview"
import $ from 'jquery';
import ColMember from './members';
import { TeamsController } from './services/teamsController';
import TreeView from '../utility/treeview';
import styles from './css/style.module.css';
import scroll from './css/scroll.module.css';
import { BsFillPeopleFill } from "react-icons/bs";
//import style from './css/vacation.module.css';
//import '../Root/css/teams/NavMenu.css';
//import { Link } from 'react-router-dom';
//import { VacationMenus } from '../Vacation/vacationMenus';
import { Link } from 'react-router-dom';
import './css/NavMenu.css';
//import TreeViewstyles from '../utility/css/style.css';
//import '../utility/treeview.jsx'


/*<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1" crossorigin="anonymous"/>*/

export class Teams extends Component {


    constructor(props) {
        super(props);

        this.state = {
            teamTreeData: [],
            team: null,
            isEditMode: null,
            members: null,
            jobLevels: null,
            prevProps: this.props,
           /* disUI: this.displayUI(), */
        }

        this.memberInfoChange = this.memberInfoChange.bind(this);
        this.saveMember = this.saveMember.bind(this);
        this.deleteMember = this.deleteMember.bind(this);
        this.addTeam = this.addTeam.bind(this);
        this.deleteTeam = this.deleteTeam.bind(this);
        this.displayRegular = this.displayRegular.bind(this);
        this.checkTeamLeader = this.checkTeamLeader.bind(this);
        this.checkAdminLength = this.checkAdminLength.bind(this);
    }

    /* resizeUI() {
        this.setState({ disUI: this.displayUI() });
    }

    componentDidMount() {
        window.addEventListener('resize', () => this.resizeUI());
    } */


    /* onClickMenu = (menu) => {
        this.props.onSelectMenu(menu);
    }

    getMenuItemClassName(isActive) {
        if (isActive) {
            return style.menuItem + " " + style.active;
        }

        return style.menuItem;
    } */


    componentDidMount() {
        this.displayRegular();
        this.displayJobLevel();

        //$('.' + style.menuicon).click(function () {
        //    $('.' + style.menuItems).toggleClass("menuOn");
        //});

        $('.hamburger').on('click', function () {
            $('#sidebar').addClass('active');
            $('.overlay').fadeIn();
        });

        //$('.overlay').on('click', function () {
        //    $('#sidebar').removeClass('active');
        //    $('.overlay').fadeOut();
        //});

        /* 이거 열면 편집을 못함 */
        $('#sidebar').on('click', function () {
            $('#sidebar').removeClass('active');
            $('.overlay').fadeOut();
        });
    }


    processLogin = () => {
        if (this.props.loginUser) {
            this.props.onLogout(this.props.loginUser);
        }
        else {
            this.props.onLogin();
        }
    }


    shouldComponentUpdate(nextProps, nextState) {
        return nextState !== this.state;
    }


    async displayRegular() {
        const datas = await TeamsController.displayRegular();
        const jsonData = this.convertToTree(datas);

        if (jsonData.length > 0) {
            this.setState({ teamTreeData: jsonData, team: jsonData[0] });
        }
        else {
            this.setState({ teamTreeData: jsonData });
        }

        this.displayRegularMember();
    }

    async displayJobLevel() {
        const jobLevels = await TeamsController.displayJobLevel();
        this.setState({ jobLevels: JSON.parse(jobLevels) });
    }

    convertToTree(datas) {
        const tree = [];
        for (const data of datas) {

            const nodeData = { ID: data.id, Name: data.name, ParentTeamID: data.parentTeamID };

            const parent = this.findParent(data, tree); // parent가 있는가?
            if (parent !== null) {
                if (!parent.children) {
                    parent.children = []
                }

                parent.children.push(nodeData); //children에 등록
            }
            else {
                tree.push(nodeData); // root에 등록
            }
        }

        return tree;
    }

    findParent(current, nodes) {
        for (const node of nodes) {
            if (current.parentTeamID === node.ID) {
                return node;
            }
            if (node.children) { // 자식노드들에서도 검색
                //const parent = arguments.callee(current, node.children); // 재귀
                const parent = this.findParent(current, node.children);
                if (parent) {
                    return parent;
                }
            }
        }
        return null;
    }

    onTreeNodeChanged = (team) => {
        if (this.state.team.ID !== team.ID) {
            this.setState({ team: team }, () => this.displayRegularMember());
        }
    }

    async displayRegularMember() {
        const arrMembers = await TeamsController.displayRegularMember(this.state.team.ID);
        this.setState({ members: JSON.parse(arrMembers) });
    }

    onChangedDeleteCheckBox(checked, rowNum) {
        this.state.members[rowNum].checked = checked;
        this.setState({ members: this.state.members });
    }

    onChangeEditMode = (checked) => {
        if (this.state.isEditMode !== checked) {
            this.setState({ isEditMode: checked });
        }

        return;
    }

    memberInfoChange = (member, index) => {
        const newArray = [...this.state.members];
        newArray[index] = member;
        newArray[index].change = 1; // 변경된거 1로 체크하기

        //this.state.members[index] = member;
        //this.state.members[index].change = 1; 
        this.setState({ members: newArray });
    }

    // 팀에 이미 팀장이 있으면 팀원으로 바꿔줌 (최상위 제외)
    checkTeamLeader(member) {
        if (this.state.team.ParentTeamID === null)
            return;

        const newArray = [...this.state.members];
        for (var i = 0; i < newArray.length; i++) {
            if (newArray[i] !== member) {
                if (newArray[i].CompanyMember.IsTeamLeader) {
                    newArray[i].CompanyMember.IsTeamLeader = false;
                    newArray[i].change = 1;
                }
            }
        }
    }

    async saveMember() {
        if (!this.state.isEditMode)
            return;

        const suc = await TeamsController.saveMember(this.state.members);
        if (suc) {
            await this.displayRegularMember();
        }
    }

    addMember = () => {
        if (!this.state.isEditMode)
            return;

        const newArray = [...this.state.members]; // 복사해서 새로운 배열을 만듦

        const newMemberInfo = { ID: -1, Name: 'new', TeamID: this.state.team.ID, JobLevelID: 1, PhoneNumber: '010' };
        const newJobLevel = { ID: 1, LevelName: '사원' };        

        let leng = newArray.length;
        newArray[leng] = {};
        newArray[leng].CompanyMember = {};
        newArray[leng].CompanyMember = newMemberInfo;
        newArray[leng].RegularTeam = this.state.team;
        newArray[leng].JobLevel = newJobLevel;
        newArray[leng].change = 1;
        this.setState({ members: newArray });

        return;
    }

    async deleteMember() {
        if (!this.state.isEditMode)
            return;

        var deleteMembers2 = [...this.state.members];
        var deleteMembers = [];

        for (var i = 0; i < this.state.members.length; i++) {
            if (this.state.members[i].checked === true) {

                let id = this.state.members[i].CompanyMember.ID;

                if (this.props.loginUser.id === id) {
                    alert('로그인된 사용자는 직접 삭제할 수 없습니다.');
                    return;
                }
                else {
                    deleteMembers.push(this.state.members[i]);

                    const idx = deleteMembers2.findIndex(
                        function (item) {
                            return item.CompanyMember.ID === id
                        })
                    deleteMembers2.splice(idx, 1);
                }
            }
        }

        if (deleteMembers.length === 0)
            return;

        const msg = deleteMembers.length + '명의 직원을 삭제하시겠습니까?'

        if (window.confirm(msg)) {
            await TeamsController.deleteMember(deleteMembers);
            this.setState({ members: deleteMembers2 })
        }
        else
            return;
    }

    async addTeam() {
        if (!this.state.isEditMode)
            return;

        await TeamsController.saveTeam(-1, 'new', this.state.team.ID);
        await this.displayRegular()
    }

    async deleteTeam() {
        if (!this.state.isEditMode)
            return;

        if (window.confirm('해당 팀 및 하위 팀에 속한 직원들도 삭제됩니다. 계속 하시겠습니까?')) {
            await TeamsController.deleteTeam(this.state.team);
            await this.displayRegular();
        } else 
            return;
    }

    async teamInfoChange(team) {        
        await TeamsController.saveTeam(team.ID, team.Name, team.ParentTeamID);
    }

    checkAdminLength(memberID) {
        for (var i = 0; i < this.state.members.length; i++) {
            if (memberID !== this.state.members[i].CompanyMember.ID) {
                if (this.state.members[i].CompanyMember.IsAdmin === true) {
                    return true;
                }
            }
        }

        return false;
    }

/*    $(document).ready(function(){

        $('edit input:checkbox').click(function (e) {
            var check = $(this).is(":checked");

            $(this).addbtn.css("background", check ? "red" : "#ffffff")

            $(e.target).addbtn.css("background", check ? "red" : "#ffffff")

        })
    })*/

    displayUI = () => {
        let displayUI = [];
        let widthSize = window.outerWidth;


        const loginMenu = this.props.loginUser ? this.props.loginUser.userID + " 로그아웃" : "로그인";

        let memberGridView = null;
        if (this.state.members !== null && this.state.members.length > 0) {
            memberGridView =
                this.state.members.map((member, index) =>
                (
                    <tr key={member.CompanyMember.ID}>
                        <td><input type="checkbox"
                            value={false}
                            onChange={(e) => this.onChangedDeleteCheckBox(e.target.checked, index)} /></td>
                        <td><span>{index + 1}</span></td>
                        <ColMember member={member} isEditMode={this.state.isEditMode} index={index} memberInfoChange={this.memberInfoChange} jobLevels={this.state.jobLevels} checkTeamLeader={this.checkTeamLeader} checkAdminLength={this.checkAdminLength} />
                    </tr>
                ))
        }

        const teamName = (this.state.team === null) ? '' : this.state.team.Name;

        /* if (widthSize <= 1024) { */
        if (widthSize < 768) { //모바일
            displayUI.push(
                <>
                    <div id="wrap">
                        <nav id="sidebar">
                            <span className={styles.manageTitle}><Link to="/vacation">휴가관리</Link></span>
                            <span className={styles.teamManageTitle}>
                                {
                                    this.props.loginUser?.isAdmin && (
                                        <span>
                                            <Link to="/teams">조직관리</Link>
                                        </span>
                                    )

                                }
                            </span>
                            {/* <span className={styles.sideTitle2}>조직관리</span> */}

                            <div id={styles.subPage}>
                                <div className={styles.edit}>
                                    <input type="checkbox" onChange={(e) => this.onChangeEditMode(e.target.checked)} checked={this.state.isEditMode} />
                                    <label>편집</label>
                                </div>
                                <div id={styles.subAside}>
                                    <div className={styles.saRht}>
                                        <div className={styles.sarSel}>
                                            <p className={styles.teamTitle}><span></span></p>
                                        </div>
                                        <div className={styles.sarEdit}>
                                            <button onClick={this.addTeam} disabled={!this.state.isEditMode}>추가</button>
                                            <button onClick={this.deleteTeam} disabled={!this.state.isEditMode}>삭제</button>
                                        </div>
                                        <TreeView id={styles.checked}
                                            teamTreeData={this.state.teamTreeData}
                                            onTreeNodeChanged={this.onTreeNodeChanged}
                                            teamInfoChange={this.teamInfoChange}
                                            isEditMode={this.state.isEditMode} />
                                    </div>
                                </div>
                            </div>
                        </nav>

                        <header className="hamburgerHeader" onClick={this.processLogin}><span className="hamburgerlogin">{loginMenu}</span></header>
                        <div className="hamburger">
                            <input type="checkbox" />
                            <div className="hamburgerlines">
                                <span className="lines line1">
                                </span>
                                <span className="lines line2">
                                </span>
                                <span className="lines line3">
                                </span>
                            </div>
                        </div>
                        <div className="overlay"></div>

                    </div>

                    <div id={styles.subCont} className={scroll.scrollbarOuter}>
                        <div className={styles.scWrap}>
                            <h4></h4>
                            <hr></hr>
                            <div className={styles.scCont}>
                                <div className={styles.scTop}>
                                    <h4>{teamName}</h4>
                                    <div className={styles.sctRht}>
                                        <button className={styles.sctDeladd} onClick={this.addMember} active={styles.active} disabled={!this.state.isEditMode}>추가</button>
                                        <button className={styles.sctDeldel} onClick={this.deleteMember} disabled={!this.state.isEditMode}>삭제</button>
                                        <button className={styles.sctDelsave} onClick={this.saveMember} disabled={!this.state.isEditMode}>저장</button>
                                    </div>
                                </div>
                                <table className={styles.scTb}>
                                    <thead>
                                        <tr>
                                            <th style={{ width: 6.5 + '%' }}></th>
                                            <th style={{ width: 5 + '%' }}>No</th>
                                            <th style={{ width: 17 + '%' }}>소속팀</th>
                                            <th style={{ width: 16 + '%' }}>이름</th>
                                            <th style={{ width: 11 + '%' }}>직급</th>
                                            <th style={{ width: 8 + '%' }}>팀장</th>
                                            <th style={{ width: 8 + '%' }}>관리자</th>
                                            <th style={{ width: 16 + '%' }}>휴대전화번호</th>
                                            <th style={{ width: 14 + '%' }}>입사일</th>
                                            <th style={{ width: 8 + '%' }}>ID</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {memberGridView}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div> 
                </>
            );

           } else if (640 <= widthSize && widthSize <= 959) { //가로 모바일
                displayUI.push(
                    <>
                        <div id="wrap">
                            <nav id="sidebar">
                                <span className={styles.manageTitle}><Link to="/vacation">휴가관리</Link></span>
                                <span className={styles.teamManageTitle}>
                                    {
                                        this.props.loginUser?.isAdmin && (
                                            <span>
                                                <Link to="/teams">조직관리</Link>
                                            </span>
                                        )

                                    }
                                </span>
                                {/* <span className={styles.sideTitle2}>조직관리</span> */}

                                <div id={styles.subPage}>
                                    <div className={styles.edit}>
                                        <input type="checkbox" onChange={(e) => this.onChangeEditMode(e.target.checked)} checked={this.state.isEditMode} />
                                        <label>편집</label>
                                    </div>
                                    <div id={styles.subAside}>
                                        <div className={styles.saRht}>
                                            <div className={styles.sarSel}>
                                                <p className={styles.teamTitle}><span></span></p>
                                            </div>
                                            <div className={styles.sarEdit}>
                                                <button onClick={this.addTeam} disabled={!this.state.isEditMode}>추가</button>
                                                <button onClick={this.deleteTeam} disabled={!this.state.isEditMode}>삭제</button>
                                            </div>
                                            <TreeView id={styles.checked}
                                                teamTreeData={this.state.teamTreeData}
                                                onTreeNodeChanged={this.onTreeNodeChanged}
                                                teamInfoChange={this.teamInfoChange}
                                                isEditMode={this.state.isEditMode} />
                                        </div>
                                    </div>
                                </div>
                            </nav>

                            <header className="hamburgerHeader" onClick={this.processLogin}><span className="hamburgerlogin">{loginMenu}</span></header>
                            <div className="hamburger">
                                <input type="checkbox" />
                                <div className="hamburgerlines">
                                    <span className="lines line1">
                                    </span>
                                    <span className="lines line2">
                                    </span>
                                    <span className="lines line3">
                                    </span>
                                </div>
                            </div>
                            <div className="overlay"></div>

                        </div>

                        <div id={styles.subCont} className={scroll.scrollbarOuter}>
                            <div className={styles.scWrap}>
                                <h4></h4>
                                <hr></hr>
                                <div className={styles.scCont}>
                                    <div className={styles.scTop}>
                                        <h4>{teamName}</h4>
                                        <div className={styles.sctRht}>
                                            <button className={styles.sctDeladd} onClick={this.addMember} active={styles.active} disabled={!this.state.isEditMode}>추가</button>
                                            <button className={styles.sctDeldel} onClick={this.deleteMember} disabled={!this.state.isEditMode}>삭제</button>
                                            <button className={styles.sctDelsave} onClick={this.saveMember} disabled={!this.state.isEditMode}>저장</button>
                                        </div>
                                    </div>
                                    <table className={styles.scTb}>
                                        <thead>
                                            <tr>
                                                <th style={{ width: 6.5 + '%' }}></th>
                                                <th style={{ width: 5 + '%' }}>No</th>
                                                <th style={{ width: 17 + '%' }}>소속팀</th>
                                                <th style={{ width: 16 + '%' }}>이름</th>
                                                <th style={{ width: 11 + '%' }}>직급</th>
                                                <th style={{ width: 8 + '%' }}>팀장</th>
                                                <th style={{ width: 8 + '%' }}>관리자</th>
                                                <th style={{ width: 16 + '%' }}>휴대전화번호</th>
                                                <th style={{ width: 14 + '%' }}>입사일</th>
                                                <th style={{ width: 8 + '%' }}>ID</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {memberGridView}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </>
            );
        } else if (768 <= widthSize && widthSize <= 1024) { //태블릿
            displayUI.push(
                <>
                    <div id={styles.subPage}>
                        <div className={styles.edit}>
                            <input type="checkbox" onChange={(e) => this.onChangeEditMode(e.target.checked)} checked={this.state.isEditMode} />
                            <label>편집</label>
                        </div>
                        <div id={styles.subAside}>
                            <div className={styles.saRht}>
                                <div className={styles.sarSel}>
                                    {/* <p className={styles.teamTitle}><BsFillPeopleFill size="40" /><span>조직관리</span></p> */}
                                    <p className={styles.teamTitle}>{/*<span>조직관리</span>*/}</p>
                                </div>
                                <div className={styles.sarEdit}>
                                    <button onClick={this.addTeam} disabled={!this.state.isEditMode}>추가</button>
                                    <button onClick={this.deleteTeam} disabled={!this.state.isEditMode}>삭제</button>
                                </div>
                                <TreeView id={styles.checked}
                                    teamTreeData={this.state.teamTreeData}
                                    onTreeNodeChanged={this.onTreeNodeChanged}
                                    teamInfoChange={this.teamInfoChange}
                                    isEditMode={this.state.isEditMode} />
                            </div>
                        </div>
                        <div id={styles.subCont} className={scroll.scrollbarOuter}>
                            <div className={styles.scWrap}>
                                <h4>조직관리</h4>
                                <hr></hr>
                                <div className={styles.scCont}>
                                    <div className={styles.scTop}>
                                        <h4>{teamName}</h4>
                                        <div className={styles.sctRht}>
                                            <button className={styles.sctDeladd} onClick={this.addMember} active={styles.active} disabled={!this.state.isEditMode}>추가</button>
                                            <button className={styles.sctDeldel} onClick={this.deleteMember} disabled={!this.state.isEditMode}>삭제</button>
                                            <button className={styles.sctDelsave} onClick={this.saveMember} disabled={!this.state.isEditMode}>저장</button>
                                        </div>
                                    </div>
                                    <table className={styles.scTb}>
                                        <thead>
                                            <tr>
                                                <th style={{ width: 5 + '%' }}></th>
                                                <th style={{ width: 5 + '%' }}>No</th>
                                                <th style={{ width: 15 + '%' }}>소속팀</th>
                                                <th style={{ width: 10 + '%' }}>이름</th>
                                                <th style={{ width: 10 + '%' }}>직급</th>
                                                <th style={{ width: 8 + '%' }}>팀장</th>
                                                <th style={{ width: 8 + '%' }}>관리자</th>
                                                <th style={{ width: 16 + '%' }}>휴대전화번호</th>
                                                <th style={{ width: 18 + '%' }}>입사일</th>
                                                <th style={{ width: 18 + '%' }}>ID</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {memberGridView}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            );

        } else if (960 <= widthSize && widthSize <= 1280) {
            displayUI.push(
                <>
                    <div id={styles.subPage}>
                        <div className={styles.edit}>
                            <input type="checkbox" onChange={(e) => this.onChangeEditMode(e.target.checked)} checked={this.state.isEditMode} />
                            <label>편집</label>
                        </div>
                        <div id={styles.subAside}>
                            <div className={styles.saRht}>
                                <div className={styles.sarSel}>
                                    {/* <p className={styles.teamTitle}><BsFillPeopleFill size="40" /><span>조직관리</span></p> */}
                                    <p className={styles.teamTitle}>{/*<span>조직관리</span>*/}</p>
                                </div>
                                <div className={styles.sarEdit}>
                                    <button onClick={this.addTeam} disabled={!this.state.isEditMode}>추가</button>
                                    <button onClick={this.deleteTeam} disabled={!this.state.isEditMode}>삭제</button>
                                </div>
                                <TreeView id={styles.checked}
                                    teamTreeData={this.state.teamTreeData}
                                    onTreeNodeChanged={this.onTreeNodeChanged}
                                    teamInfoChange={this.teamInfoChange}
                                    isEditMode={this.state.isEditMode} />
                            </div>
                        </div>
                        <div id={styles.subCont} className={scroll.scrollbarOuter}>
                            <div className={styles.scWrap}>
                                <h4></h4>
                                <hr></hr>
                                <div className={styles.scCont}>
                                    <div className={styles.scTop}>
                                        <h4>{teamName}</h4>
                                        <div className={styles.sctRht}>
                                            <button className={styles.sctDeladd} onClick={this.addMember} active={styles.active} disabled={!this.state.isEditMode}>추가</button>
                                            <button className={styles.sctDeldel} onClick={this.deleteMember} disabled={!this.state.isEditMode}>삭제</button>
                                            <button className={styles.sctDelsave} onClick={this.saveMember} disabled={!this.state.isEditMode}>저장</button>
                                        </div>
                                    </div>
                                    <table className={styles.scTb}>
                                        <thead>
                                            <tr>
                                                <th style={{ width: 5 + '%' }}></th>
                                                <th style={{ width: 5 + '%' }}>No</th>
                                                <th style={{ width: 15 + '%' }}>소속팀</th>
                                                <th style={{ width: 10 + '%' }}>이름</th>
                                                <th style={{ width: 10 + '%' }}>직급</th>
                                                <th style={{ width: 8 + '%' }}>팀장</th>
                                                <th style={{ width: 8 + '%' }}>관리자</th>
                                                <th style={{ width: 16 + '%' }}>휴대전화번호</th>
                                                <th style={{ width: 18 + '%' }}>입사일</th>
                                                <th style={{ width: 18 + '%' }}>ID</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {memberGridView}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </>
            );
        } else if (widthSize >= 1025) {
            displayUI.push(
                <>
                    <div id={styles.subPage}>
                            <div className={styles.edit}>
                                <input type="checkbox" onChange={(e) => this.onChangeEditMode(e.target.checked)} checked={this.state.isEditMode} />
                                <label>편집</label>
                            </div>
                            <div id={styles.subAside}>
                                <div className={styles.saRht}>
                                    <div className={styles.sarSel}>
                                    {/* <p className={styles.teamTitle}><BsFillPeopleFill size="40" /><span>조직관리</span></p> */}
                                    <p className={styles.teamTitle}>{/*<span>조직관리</span>*/}</p>
                                    </div>
                                    <div className={styles.sarEdit}>
                                        <button onClick={this.addTeam} disabled={!this.state.isEditMode}>추가</button>
                                        <button onClick={this.deleteTeam} disabled={!this.state.isEditMode}>삭제</button>
                                    </div>
                                    <TreeView id={styles.checked}
                                        teamTreeData={this.state.teamTreeData}
                                        onTreeNodeChanged={this.onTreeNodeChanged}
                                        teamInfoChange={this.teamInfoChange}
                                        isEditMode={this.state.isEditMode} />
                                </div>
                            </div>
                            <div id={styles.subCont} className={scroll.scrollbarOuter}>
                                <div className={styles.scWrap}>
                                    <h4>조직관리</h4>
                                    <hr></hr>
                                    <div className={styles.scCont}>
                                        <div className={styles.scTop}>
                                            <h4>{teamName}</h4>
                                            <div className={styles.sctRht}>
                                                <button className={styles.sctDeladd} onClick={this.addMember} active={styles.active} disabled={!this.state.isEditMode}>추가</button>
                                                <button className={styles.sctDeldel} onClick={this.deleteMember} disabled={!this.state.isEditMode}>삭제</button>
                                                <button className={styles.sctDelsave} onClick={this.saveMember} disabled={!this.state.isEditMode}>저장</button>
                                            </div>
                                        </div>
                                        <table className={styles.scTb}>
                                            <thead>
                                                <tr>
                                                    <th style={{ width: 5 + '%' }}></th>
                                                    <th style={{ width: 5 + '%' }}>No</th>
                                                    <th style={{ width: 15 + '%' }}>소속팀</th>
                                                    <th style={{ width: 10 + '%' }}>이름</th>
                                                    <th style={{ width: 10 + '%' }}>직급</th>
                                                    <th style={{ width: 8 + '%' }}>팀장</th>
                                                    <th style={{ width: 8 + '%' }}>관리자</th>
                                                    <th style={{ width: 16 + '%' }}>휴대전화번호</th>
                                                    <th style={{ width: 18 + '%' }}>입사일</th>
                                                    <th style={{ width: 18 + '%' }}>ID</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {memberGridView}
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div> 
                
                </>
            );
        } else {
            displayUI.push(
               <></>
            );
        }
        return displayUI;
    }


    render() {

        /* console.log('render');
        let memberGridView = null;
        if (this.state.members !== null && this.state.members.length > 0) 
        {
            memberGridView =                
            this.state.members.map((member, index) =>
                (
                    <tr key={member.CompanyMember.ID}>
                        <td><input type="checkbox"
                                value={false}
                                onChange={(e) => this.onChangedDeleteCheckBox(e.target.checked, index)} /></td>
                        <td><span>{index + 1}</span></td>
                        <ColMember member={member} isEditMode={this.state.isEditMode} index={index} memberInfoChange={this.memberInfoChange} jobLevels={this.state.jobLevels} checkTeamLeader={this.checkTeamLeader} checkAdminLength={this.checkAdminLength}/>
                    </tr>
                ))
        }

        const teamName = (this.state.team === null) ? '' : this.state.team.Name; */

        /*export const IconHome () => <Icon icon={home} />*/


        /* setTimeout(() => { this.resizeUI() }, 500);
        let displayUI = this.state.disUI; */
        let displayUI = this.displayUI(); 

        return (
            <>
              {displayUI}
            </>
        );
    }
}
