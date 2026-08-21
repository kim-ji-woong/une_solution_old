import Commands from "./commands";
import { JsonManager } from './jsonManager';

export class TeamEditController {

    static async requestTemporaryMembers() {
        try {
            const jsonData = JsonManager.makeRequestTemporaryMembers();

            const res = await fetch('/TeamEditor/TeamEdit/RequestData', {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: jsonData
            });


            if (res.ok) {
                const result = await res.json();

                if (result.success) {
                    return [result.temporaryMemberInfos, ""];
                }
                else {
                    return [null, result.message];
                }
            }
        }
        catch (e) {
            console.log(e);
        }

        return [null, "requestTemporaryMembers 실패"];
    }

    static async DisplayRegular() {
        try {
            const response = await fetch('TeamEditor/TeamEdit/DisplayRegular');
            const datas = await response.json();
            const jsonData = this.convertToTree(datas);

            return jsonData;
        } catch (e) {
            console.log(e);
        }
    }

    static async GetRegular() {
        try {
            const response = await fetch('TeamEditor/TeamEdit/DisplayRegular');
            const datas = await response.json();

            return datas;
        } catch (e) {
            console.log(e);
        }
    }
	
	static async DisplayTemporary(isNormal) {
        try {
            const response = await fetch('TeamEditor/TeamEdit/DisplayTemporary', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({IsNormal: isNormal })
            });
            const datas = await response.json();
            const jsonData = this.convertToTree(datas);

            return jsonData;
        } catch (e) {
            console.log(e);
        }
    }

    static convertToTree(datas) {
        const tree = [];
        for (const data of datas) {

            const nodeData = {
                ID: data.id, Name: data.teamName, ParentTeam: null, ParentTeamID: data.parentTeamID
            };

            const parent = this.findParent(data.parentTeamID, tree); // parent가 있는가?
            if (parent !== null) {
                if (!parent.Children) {
                    parent.Children = []
                }

                nodeData.ParentTeam = { ID: parent.ID, Name: parent.Name, ParentTeam: parent.ParentTeam, ParentTeamID: parent.ParentTeamID }; // 부모 객체 등록
                parent.Children.push(nodeData); // Children에 등록
            }
            else {
                tree.push(nodeData); // root에 등록
            }
        }

        return tree;
    }

    static findParent(current, nodes) {
        for (const node of nodes) {
            if (current === node.ID) {
                return node;
            }
            if (node.Children) { // 자식노드들에서도 검색
                const parent = this.findParent(current, node.Children);
                if (parent) {
                    return parent;
                }
            }

        }
        return null;
    }

    static findNode(srcNode, targetID) {
        
        if (srcNode.ID === targetID) {
            return srcNode;
        }

        if (srcNode.Children) { // 자식노드들에서도 검색
            for (var i = 0; i < srcNode.Children.length; i++) {
                const childNode = this.findNode(srcNode.Children[i], targetID);
                if (childNode) {
                    return childNode;
                }
            }
        }
        
        return null;
    }

    static async findChild(targetID, src, arr) {
        if (src.length === 0)
            return;

        for (var i = 0; i < src.length; i++) {
            if (targetID === src[i].ParentTeamID) {
                arr.push({ ID: src[i].ID, TeamName: src[i].Name, ParentTeamID: src[i].ParentTeamID });
            }
            if (src[i].Children)
                this.findChild(src[i].ID, src[i].Children, arr);
        }
    }

    static async DisplayRegularMember() {
        const response = await fetch('TeamEditor/TeamEdit/DisplayRegularMember');
        if (response.ok) {
            const arrMembers = await response.text();
            return JSON.parse(arrMembers);
        }

        return null;
    }


    static async GetJobLevels() {
        const response = await fetch('TeamEditor/TeamEdit/GetJobLevels');
        if (response.ok) {
            const arrJobLevels = await response.text();
            return JSON.parse(arrJobLevels);
        }

        return null;
    }

    static async GetJobPositions() {
        const response = await fetch('TeamEditor/TeamEdit/GetJobPositions');
        if (response.ok) {
            const arrJobPositions = await response.text();
            return JSON.parse(arrJobPositions);
        }

        return null;
    }

    static async displayTemporaryMember(TemporaryID, isNormal) {
        if (TemporaryID == null || isNormal == null)
            return null;

        const response = await fetch('TeamEditor/TeamEdit/DisplayTemporaryMember', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify({ ID: TemporaryID, IsNormal: isNormal })
        });

        if (response.ok) {
            const arrMembers = await response.text();
            return JSON.parse(arrMembers);
        }

        return null;
    }

    static async saveAddRegularTeam(isRedo, cmd) {
        try {
            const response = await fetch('TeamEditor/TeamEdit/SaveAddRegularTeam', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    IsRedo: isRedo,
                    Key: cmd.Key,
                    ID: cmd.ID,
                    Name: cmd.Name,
                    ParentTeamID: cmd.Parent.ID
                })
            });

            if (response.ok) {                
                if (isRedo) {
                    const res = await response.json();
                    Commands.SetHistroyTeamID(cmd.ID, res.nNewID);
                    // Command에 기록된 ID를 DB에 저장된 ID로 업데이트한다
                    const findNode = await this.findParent(cmd.Parent.ID, cmd.Data);
                    for (var i = 0; i < findNode.Children.length; i++) {
                        if (findNode.Children[i].ID === cmd.ID) {
                            findNode.Children[i].ID = res.nNewID;
                            cmd.ID = res.nNewID;
                            break;
                        }
                    }
                }
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async saveRemoveRegularTeam(isRedo, cmd) {
        try {
            const response = await fetch('TeamEditor/TeamEdit/SaveRemoveRegularTeam', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    IsRedo: isRedo,
                    Key: cmd.Key,
                    ID: cmd.ID,
                    DeleteTeams: cmd.DeleteTeams,
                    DeleteMembers: cmd.DeleteMembers
                })
            });

            if (response.ok) {
                
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async saveChangeRegularTeamInfo(isRedo, cmd) {
        try {
            const response = await fetch('TeamEditor/TeamEdit/SaveChangeRegularTeamInfo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    IsRedo: isRedo,
                    Key: cmd.Key,
                    ID: cmd.ID,
                    NewData: cmd.NewData,
                    OrgData: cmd.OrgData
                })
            });

            if (response.ok) {
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async saveChangeRegularMemeberInfo(isRedo, cmd) {
        try {                                                                        
            const response = await fetch('TeamEditor/TeamEdit/SaveChangeRegularMemeberInfo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    IsRedo: isRedo,
                    Key: cmd.Key,
                    InfoType: cmd.InfoType,
                    OrgData: cmd.OrgData,
                    ChgData: cmd.ChgData,
                    ID: cmd.ID,
                    RegularID: cmd.RegularID,
                    MemberName: cmd.MemberName,
                    MemberID: cmd.MemberID,
                    OfficePhoneNumber: cmd.OfficePhoneNumber,
                    PhoneNumber: cmd.PhoneNumber,
                    JobLevelID: cmd.JobLevelID,
                    JobPositionID: cmd.JobPositionID
                })
            });

            if (response.ok) {
                const res = await response.json();
                
                if (cmd.InfoType === 'New') {
                    // 새로 추가된 직원의 DB ID를 넣는다.
                    // Command에 기록된 ID를 DB에 저장된 ID로 업데이트한다
                    for (var i = 0; i < cmd.Data.length; i++) {
                        if (cmd.Data[i].ID === cmd.ID) {
                            cmd.Data[i].ID = res.nNewID;
                            cmd.ChgData = res.nNewID;
                            cmd.ID = res.nNewID;
                            break;
                        }
                    }
                }
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async saveRemoveRegularMemeber(isRedo, cmd) {
        try {
            const response = await fetch('TeamEditor/TeamEdit/SaveRemoveRegularMemeber', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({
                    IsRedo: isRedo,
                    Key: cmd.Key,                    
                    ID: cmd.ID,
                    Member: cmd.Member
                })
            });

            if (response.ok) {
                
            }
        } catch (e) {
            console.log(e);
        }
    }

    static async saveUpdateData(updateData) {

        if (updateData === null ||
            updateData.addRegular === null || updateData.addRegular === undefined ||
            updateData.updateRegular === null || updateData.updateRegular === undefined ||
            updateData.removeRegular === null || updateData.removeRegular === undefined ||
            updateData.addRegularMembers === null || updateData.addRegularMembers === undefined ||
            updateData.updateRegularMembers === null || updateData.updateRegularMembers === undefined ||
            updateData.removeRegularMembers === null || updateData.removeRegularMembers === undefined ||
            updateData.addTemporary === null || updateData.addTemporary === undefined ||
            updateData.updateTemporary === null || updateData.updateTemporary === undefined ||
            updateData.removeTemporary === null || updateData.removeTemporary === undefined ||
            updateData.addTemporaryEmergency === null || updateData.addTemporaryEmergency === undefined ||
            updateData.updateTemporaryEmergency === null || updateData.updateTemporaryEmergency === undefined ||
            updateData.removeTemporaryEmergency === null || updateData.removeTemporaryEmergency === undefined ||
            updateData.addTemporaryMembers === null || updateData.addTemporaryMembers === undefined ||
            updateData.updateTemporaryMembers === null || updateData.updateTemporaryMembers === undefined ||
            updateData.removeTemporaryMembers === null || updateData.removeTemporaryMembers === undefined)
            return [false, "제대로 된 데이터가 아닙니다."];

        try {
            const response = await fetch('TeamEditor/TeamEdit/SaveUpdateData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify(updateData)
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    return [true, ""];
                }
                else {
                    return [false, result.message];
                }
            }

        } catch (e) {
            console.log(e);
            return [false, e];
        }

        return [false, "saveUpdateData 실패."];
    }
}