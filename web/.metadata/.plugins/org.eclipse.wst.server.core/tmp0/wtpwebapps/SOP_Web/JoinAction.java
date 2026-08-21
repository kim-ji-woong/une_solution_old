package net.member.action;
import java.io.PrintWriter;
import java.sql.Timestamp;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import net.member.db.MemberBean;
import net.member.db.MemberDAO;
public class JoinAction implements Action{
	public ActionForward execute(HttpServletRequest request, HttpServletResponse response) 
	throws Exception{
		request.setCharacterEncoding("euc-kr");		
		MemberDAO memberdao=new MemberDAO();
		MemberBean dto=new MemberBean();
		ActionForward forward=null;		
		dto.setMEMBER_NAME(request.getParameter("UserName"));
		dto.setMEMBER_ID(request.getParameter("UserID"));
		dto.setMEMBER_PW(request.getParameter("Password"));
		dto.setMEMBER_NUMBER(request.getParameter("MemberID"));
		dto.setMEMBER_GROUP(request.getParameter("TeamID"));
		
		dto.setMEMBER_ADMIN(0);
		dto.setMEMBER_JOIN_DATE(new Timestamp(System.currentTimeMillis()));		
		memberdao.insertMember(dto);		
		response.setContentType("text/html; charset=euc-kr");
		PrintWriter out = response.getWriter();
		out.println("<script>");
		out.println("alert('등록 성공하였습니다.');");
		out.println("location.href='./join_Result.html';");
		out.println("</script>");			
		out.close();	
		return forward;
	}
}