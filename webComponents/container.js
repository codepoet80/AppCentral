//AppCentral Container functions

function getSkin()
{
	var strStyle = "<STYLE>";
	for (var i=0;i<self.document.styleSheets.length;i++)
		{
			if (!document.all)
			{
				for (var y=0;y<self.document.styleSheets[i].cssRules.length;y++)
				{
					strStyle+= self.document.styleSheets[i].cssRules[y].selectorText + "\t {" + self.document.styleSheets[i].cssRules[y].style.cssText + "}\n";
				}
			}
			else
			{
				for (var y=0;y<self.document.styleSheets[i].rules.length;y++)
				{
					strStyle+= self.document.styleSheets[i].rules[y].selectorText + "\t {" + self.document.styleSheets[i].rules[y].style.cssText + "}\n";
				}
			}
		}
	strStyle+="</STYLE>";
	return strStyle;
}

function changeView(src, moduleName, openType)
{
	if (src != "" && src != null && src != undefined)
	{
        if (src.indexOf("?") != -1)
	        src = src + "&openType=window";
	    else
	        src = src + "?openType=contained";
	}
    if (!openType || openType != "window")
    {
        var closeOK = true;
        try
        {
            closeOK = window.view.closeView();
        }
        catch(ex)
        {
            //no response, must be OK to close
        }
        if (closeOK == true)
        {
	        if (src != "" && src != null && src != undefined)
	        {
		        containerCleanup();
		        window.title = "AppCentral :: " + moduleName;
		        window.toolbar.openModule(moduleName);
		        window.view.document.location = "";
		        window.view.document.location = src;
	        }
	        else
	        {
		        containerCleanup();
	        }
	    }
	}
	else
	{
	    var childWin = window.open(window.rootDir + src, moduleName, "toolbar=no,menubar=no,resizable=yes,location=no");
	}
}

function containerCleanup()
{
	window.title = "AppCentral";
	window.toolbar.openModule();
	window.view.document.location = "";
	window.view.document.location = window.rootDir + "webComponents/acNone.htm";
	document.getElementById("main").rows = "0,*";
	window.picker.document.location = window.rootDir + "webComponents/acBlank.htm";
	window.toolbar.clearToolbars();
}

function shutDown()
{
    containerCleanup();
	try
	{
		window.close();
	}
	catch(e)
	{
		alert ("It is now safe to shut down");
	}
}

function toggleFrame(frame)
{
	if (frame == "taskbar")
	{
		if(document.getElementById("taskview").cols == "0,*")
			document.getElementById("taskview").cols = "110,*";
		else
			document.getElementById("taskview").cols = "0,*";
	}
}

function showInfo()
{
    showMessageWin("INFO", "AppCentral web application container. Powered by jsObjects.\nVisit www.jonandnic.com for more information.");
	//alert ("AppCentral web application container.");
}

function loadFrame(frame, src)
{
	if (frame == "picker")
	{
		document.getElementById("main").rows = "30,*";
		window.picker.document.location = src;
	}
}

function loadToolbar(tbSrc, tbWidth)
{
	window.toolbar.addToolbar(tbSrc, tbWidth);
}

function update(obj)
{
	var logObj = new Object();
	logObj.type = "UPDATE";
	logObj.description = obj.sender;
	logObj.data = obj.itemID;
	logEvent(logObj);
		
	for (var f=0;f<window.frames.length;f++)
	{		
		try
		{
			window.frames[f].update(obj);
		}
		catch(e)
		{
			//
		}
	}
}

var eventWin;
var lastEvent;
function logEvent(eventObj)
{
	if (eventWin)
	{
		try
		{
			eventWin.update(eventObj);
		}
		catch(e)
		{
		    lastEvent = eventObj;
			eventWin = null;
		}
	}
	else
	    lastEvent = eventObj;
}

function showEventWin()
{	
	if (!eventWin)
	{
		eventWin = window.open("acEventLog.htm", "eventWin", "width=640, height=400, scrollbars=yes");
    }
	else
	{
		try
		{
			eventWin.focus();
		}
		catch(e)
		{
			eventWin = null;
		}
	}
}

function showErrorMessage(error, message)
{
	if (error != "SUCCESS" && message != "SUCCESS")
	{
        showMessageWin(error, message);
            
        var logObj = new Object();
        logObj.type = error;
        logObj.description = message;
        logObj.data = window.lastError;
        logEvent(logObj);
        window.lastError = logObj;
     }
}

var m_oPopup;
var ti;
function showMessageWin(type, text)
{
    var msgTitle;
	if (type == null || type == undefined || type == "PARSE ERROR")
	    type = "error";
	if (type.toLowerCase() == "info")
	    msgTitle = "Information";
	else if (type.toLowerCase() == "warning")
	    msgTitle = "Warning";
	else if (type.toLowerCase() == "fatal" || type.toLowerCase() == "error")
	{
	    type = "fatal";
	    msgTitle = "Error";
	    window.frames.toolbar.networkStatus("error");
	}
	        
    if (browser == "IE")
    {
        m_oPopup = self.toolbar.createPopup();
	    self.toolbar.m_oPopup = m_oPopup;
	    var strStyle = "<STYLE>";
	    for (var i=0;i<self.toolbar.document.styleSheets.length;i++)
	    {
		    for (var y=0;y<self.toolbar.document.styleSheets[i].rules.length;y++)
		    {

			    if (self.toolbar.document.styleSheets[i].rules[y].selectorText != "3D" && self.toolbar.document.styleSheets[i].rules[y].selectorText.indexOf('VPML') != 0)
				    strStyle+= self.toolbar.document.styleSheets[i].rules[y].selectorText + "\t {" + self.toolbar.document.styleSheets[i].rules[y].style.cssText + "}\n";
		    }
	    }
	    strStyle+="</STYLE>";
    	
	    var strResult = "<HTML><HEAD>" + strStyle + "</HEAD><BODY topmargin=\"0\" leftmargin=\"0\" class=\"slip\" style='overflow:hidden;' ><table cellpadding=6><tr><td><img src='/webImages/none.gif' width='" + getWidth(text) * 7 + "' height='1'><br>" 
	    strResult += "<table ><tr><td>&nbsp;<img src='" + self.rootDir + "/webImages/" + type + ".gif' width=32 height=32 align=absmiddle>&nbsp;&nbsp;&nbsp;</td><td align='left' width='100%'><b>" + msgTitle + "</b></td></tr>"
	    strResult += "<tr><td></td><td align='left'>" + text + "<br>&nbsp;</td></tr></table>"
	    strResult += "</td></tr></table></BODY></HTML>";
	    m_oPopup.document.write(strResult);

	    var oPopupBody = m_oPopup.document.body;
	    m_oPopup.show(0, 0, 0, 0,document.body);
	    realHeight = oPopupBody.scrollHeight-1;
	    heightDiff = realHeight + ""
	    heightDiff = heightDiff.slice(heightDiff.length -1)
	    realHeight = realHeight - heightDiff;
	    realWidth = oPopupBody.scrollWidth;
	    nLeft = self.document.body.clientWidth / 2 - realWidth / 2;
	    m_oPopup.hide();
	    winheight = 0;
	    si = setInterval("scalePopup('unroll')", 1);
	    clearTimeout(ti);
	    //ti=setTimeout("m_oPopup.hide()", 20000);
	    winheight = 0;
	}
	else
	{
	    alert (msgTitle + "\n\n" + text);
	}
}

function getWidth(text)
{
	var tl = text.split("<br>");
	var longest=0;
	for (var i=0;i<tl.length;i++)
	{
		if (tl[i].length > longest)
			longest = tl[i].length;
	}		
	if (longest == 0 || text.indexOf("<br>") == -1)
	{
		longest = 35;
	}
	return longest;
}

function scalePopup(direction)
{
	if (winheight<realHeight)
	{
		winheight +=10;
		m_oPopup.hide();
		showPopup();
	}		
	else
	{
		winheight = realHeight + parseInt(heightDiff);
		showPopup();
		clearInterval(si);
	}
}

function showPopup()
{
	m_oPopup.show(nLeft, 1, realWidth, winheight, document.body)	
}

function hidePopup()
{
	try
	{
	window.top.m_oPopup.hide();
	}
	catch(e)
	{
	//alert (e.description);
	}
}

window.hyperlinkObj;
function hyperlink(src, moduleName, updateObj, openType)
{
    var logObj = new Object();
	logObj.type = "HYPERLINK";
	logObj.description = moduleName + " " + src;
	logObj.data = HTMLEncode(serializeObject(updateObj));
	logEvent(logObj);
	
    window.hyperlinkObj = updateObj;
    changeView(src + "?hyperlink=true", moduleName, openType);
}