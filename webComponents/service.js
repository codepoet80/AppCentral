// AppCentral Web Service Consumption Functions
var webservice = new Object();
var wspath;
window.lastRequest = "";
window.lastResponse = "";
window.lastError = "";

function initws()
{
    
	try
	{
		wspath = document.location + "";
		if (window.debugMode == "true")
			wspath = replace(wspath, document.location.host + "", document.location.host + ":8080");
		var wspath = wspath.split("?");
		wspath = wspath[0];
		wspath = replace(wspath, "appcentral.htm", "");
		window.rootDir = replace(wspath, ":8080", "");
		wspath = wspath + "appcentralservice.asmx";
		window.status = 'Connected to: ' + wspath;
		webservice = wsbindNew("webservice", wspath);
		if (!webservice)
			alert ("Unable to bind to a remote webservice (the webservice host is different than this page host).\nIf your sure you want to allow this interaction, please make the appropriate security setting changes in your browser.");
		else
		{
			
		}
	}
	catch(e)
	{
		alert ("Your browser is not capable of loading the modules");
	}
}

function supportForm(Type, query, data, callBack)
{
	window.lastError = "";
	window.frames.toolbar.networkStatus("send");

	var logObj = new Object();
	logObj.type = "TRAFFIC";
	logObj.description = "Request";
	logObj.data = webservice.Support(Type, query, HTMLEncode(data), callBack);
	logEvent(logObj);
		
	return logObj.data;
}

function getForm(Type, query, data, callBack)
{
	window.lastError = "";
	window.frames.toolbar.networkStatus("send");
	
	var logObj = new Object();
	logObj.type = "TRAFFIC";
	logObj.description = "Request";
	logObj.data = webservice.Get(Type, query, HTMLEncode(data), callBack);
	logEvent(logObj);
	
	return logObj.data;
}

function postForm(Type, query, data, callBack)
{
	window.lastError = "";
	window.frames.toolbar.networkStatus("send");
	var logObj = new Object();
	logObj.type = "TRAFFIC";
	logObj.description = "Request";
	logObj.data = webservice.Post(Type, query, HTMLEncode(data), callBack);
	logEvent(logObj);
	
	return logObj.data;
}

function listItems(Type, query, data, callBack)
{
	window.lastError = "";
	window.frames.toolbar.networkStatus("send");
	var logObj = new Object();
	logObj.type = "TRAFFIC";
	logObj.description = "Request";
	logObj.data = webservice.List(Type, query, HTMLEncode(data), callBack);
	logEvent(logObj);
	
	return logObj.data;
}

function decodeResponse(responseText, forOutput)
{
	window.frames.toolbar.networkStatus("receive");
	var logObj = new Object();
	logObj.type = "TRAFFIC";
	logObj.description = "Response";
	logObj.data = responseText;
	logEvent(logObj);
	var response = "";
	try
	{
		var findNode = "";
		if (responseText.indexOf("SupportResult") != -1)
			findNode = "SupportResult";
		if (responseText.indexOf("GetResult") != -1)
			findNode = "GetResult";
		if (responseText.indexOf("PostResult") != -1)
			findNode = "PostResult";
        if (responseText.indexOf("ListResult") != -1)
			findNode = "ListResult";

		if (findNode == "")
		    return handleMessages(responseText);
		
		if (browser == "IE" && findNode != "")
		{
			var xmlDoc = new ActiveXObject("MSXML2.DOMDocument");
			xmlDoc.async = false;
			if (xmlDoc.loadXML(responseText))
			{
				findNode = "//" + findNode;
				response = xmlDoc.selectSingleNode(findNode);
				if (response != null && response.childNodes.length > 0)
					response = response.firstChild.xml;
				else
					response = "";
			}
			else
				response = "";
		}
		else if (browser == "MOZ" && findNode != "")
		{
			xmlParser = new DOMParser();
			xmlDoc = xmlParser.parseFromString(responseText, 'text/xml');
			var nodes = xmlDoc.getElementsByTagName(findNode);
			if (nodes.length > 0)
			{
				if (nodes[0].childNodes.length > 0)
					response = nodes[0].firstChild.nodeValue;
				else
					response = "";
			}
			else
				response = "";
		}
		response = xmlDecode(response);
		response = replace(response, "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
		response = replace(response, "<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
		if (!forOutput)
		{
		    response = replace(response, "&", "&amp;");
		    response = replace(response, "#", "&#x0023;");
		    response = replace(response, "$", "&#x0024;");
		    response = replace(response, "%", "&#x0025;");
		    response = replace(response, "@", "&#x0040;");
		}
		setTimeout("window.frames.toolbar.networkStatus('')", 1000);
	}
	catch(e)
	{
		showErrorMessage("error", "Response could not be handled");
		return false;
	}
	handleMessages(response);
	return response;
}

function handleMessages(response)
{
    if (browser == "IE")
	{
		var xmlDoc = new ActiveXObject("MSXML2.DOMDocument");
		xmlDoc.async = false;
		if (xmlDoc.loadXML(response))
		{
			errors = xmlDoc.selectNodes("//error");
			for (var e=0;e<errors.length;e++)
			{
			    var type = errors[e].childNodes[0].text;
			    var msg = errors[e].childNodes[2].text;
			    showErrorMessage(type, msg);
			    return false;
			}
			faults = xmlDoc.selectNodes("//faultstring");
			for (var f=0;f<faults.length;f++)
			{
			    var msg = faults[f].childNodes[0]
			    msg = msg.text;
			    showErrorMessage("error", msg);
			    return false;
			}
		}
	}
	else if (browser == "MOZ")
	{
		xmlParser = new DOMParser();
		xmlDoc = xmlParser.parseFromString(response, 'text/xml');
		var errors = xmlDoc.getElementsByTagName("error");
		for (var e=0;e<errors.length;e++)
		{
		    var type = errors[e].childNodes[0];
    	    type = type.firstChild.nodeValue;
    	    showErrorMessage(type, errors[e].childNodes[2]);
    	    return response;
		}
		var faults = xmlDoc.getElementsByTagName("faultstring");
		for (var f=0;f<faults.length;f++)
		{
		    var type = faults[f].childNodes[0]
    	    var message = type.firstChild.nodeValue;
    	    type = "FAULT";
    	    showErrorMessage(type, message);
    	    return false;
		}
	}
}

