ALTER PROCEDURE [dbo].[SendNewUserAlert]  

@inCountryCode nvarchar(255),  

@inClientId nvarchar(255),  

@inDistrict nvarchar(255),     

@inName nvarchar(255),   

@inEmail nvarchar(255),  

@inCity nvarchar(255),   

@inUserHandle nvarchar(255),   

@inPassword nvarchar(255)  

  

AS  

BEGIN  

-- SET NOCOUNT ON added to prevent extra result sets from  

-- interfering with SELECT statements.  

SET NOCOUNT ON;  

  

DECLARE @Comments VARCHAR(500)  

DECLARE @tmpBody VARCHAR(2000)  

DECLARE @tmpSubject VARCHAR(500)  

DECLARE @tmpLoginURL VARCHAR(500)  

           DECLARE @tmpLogoutURL VARCHAR(500)  

  

-- Read the client globals from the persistent memory  

Select @tmpLogoutURL = isnull(logoutURL, 'https://cisanalytic.com') from Clients with (nolock) where clientid = @inClientId  

Select @tmpLoginURL = isnull(loginURL, '') from Clients with (nolock) where clientid = @inClientId  

Select TOP 1 @tmpLoginURL = [LoginURL] FROM [Fixit].[dbo].[Clients] with (nolock) where clientid = @inClientId and CountryCode = @inCountryCode  

  

-- Compose the email body  

set @tmpBody =  N'Dear ' + @inName + ',<br>'  

set @tmpBody = @tmpBody + N'Welcome to the Fixit application. Thank you for joining the Fixit team in the ' + @inDistrict + ' district to report the local issues to teh local government.<br><br>   '   

set @tmpBody = @tmpBody + N'We thank you in advance and wish you a nice day!<br>'  

set @tmpBody = @tmpBody + '<br>'  

set @tmpBody = @tmpBody + '<table border="1">'  

set @tmpBody = @tmpBody + N'<tr>Application Login URL: ' + @tmpLoginURL + '     <br></tr>'  

set @tmpBody = @tmpBody + N'<tr>Username: ' + @inUserHandle + '     <br></tr>'   

set @tmpBody = @tmpBody + '</table>'  

set @tmpSubject = N'Fixit ' + @inCity + ' New User: ' + '  ' + @inDistrict  + ' district'  

  

-- Invoke the local email send procedure to send out the composed message  

EXEC msdb.dbo.sp_send_dbmail @profile_name='CISAnalytic Mail', @recipients= @inEmail, @subject= @tmpSubject, @body = @tmpBody,  @body_format = 'HTML' ;  

END  