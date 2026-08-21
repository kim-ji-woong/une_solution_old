package kr.co.unes.data;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class ClientDataManager {
	
	private static final Logger logger = LoggerFactory.getLogger(ClientDataManager.class);
	private static ClientDataManager instance = null;
	
	public synchronized static ClientDataManager getInstance()
	{
		if( instance == null)
			instance = new ClientDataManager();
		return instance;
	}

	private Map<String, ClientData> mClientMap = new HashMap<String,ClientData>();
	
	public ClientDataManager() {
		logger.info("Create Client Data Manager");
	}
	
	public synchronized ClientData AddClientData(HttpServletRequest request, HttpSession session) {
		
		String szSessionID = session.getId();
		ClientData data = new ClientData(request, session);
		if(!mClientMap.containsKey(szSessionID))
			mClientMap.put(szSessionID, data);
		return data;
	}

	public synchronized ClientData FindClientData(String szSessionID) {
		if(mClientMap.containsKey(szSessionID))
			return mClientMap.get(szSessionID);
		return null;
	}	
}
