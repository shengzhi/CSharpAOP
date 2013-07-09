
function  GetWebFormData()
{

 MSXML2.XMLHTTPClass oxmlHttp = new MSXML2.XMLHTTPClass();  

 oxmlHttp.open("Get", url, false, null, null);  

 oxmlHttp.setRequestHeader("Referer", url);  

 oxmlHttp.send("0");  

 if (oxmlHttp.readyState == 4 && oxmlHttp.status == 200)  
 {  
    return (Byte[])oxmlHttp.responseBody;  
 } 
}

