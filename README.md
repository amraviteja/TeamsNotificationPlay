# TeamsNotificationPlay
This sample demonstrates using the Microsoft Graph APIs for Microsoft teams to send notifications in private channels using Public Client App and Client Credentials Provider


## Nuget packages

Microsoft.Extensions.Configuration ===> Version# 3.1.0  
Microsoft.Extensions.Configuration.Binder ===> Version# 3.1.0  
Microsoft.Extensions.Configuration.FileExtensions ===> Version# 3.1.0  
Microsoft.Extensions.Configuration.Json ===> Version# 3.1.0  
Microsoft.Graph ===> Version# 3.25.0
Microsoft.Graph.Auth ===> Version# 1.0.0-preview.6
Microsoft.Identity.Client ===> Version# 4.27.0
Microsoft.Graph.Auth ===> Version# 1.0.0-preview.6
Microsoft.Identity.Client ===> Version# 4.27.0

## Build and run

To run, you'll need to register your application.

1. Sign into the Azure [app registration portal](https://go.microsoft.com/fwlink/?linkid=2083908) using either your personal or work or school account.

2. Choose **New registration** near the top.

3. Enter a name for the app. Under **Supported account types**, select **Accounts in this organizational directory (Single tenant)**.

4. Copy the value for **Application (client) ID**. This is the unique identifier for your app.

5. Navigate to the **Authentication** page.
   Under **Advanced settings**, find the **Allow public client flows** section. 
   Set **Enable the following mobile and desktop flows**  to **Yes**.
   Choose **Save** at the top.
6. Under **Manage** on the left-hand pane, click **API permissions** and then **Add a new permission**. Select **Microsoft Graph** and then **Delegated permissions**.
   Add following permissions
   User.Read.All
   ChannelMessage.Send
   ChannelSettings.ReadWrite.All
   ChannelMember.ReadWrite.All
   Channel.Create
   **Note: Since we are using client credentials provider for authorization, admin has to conset for permissions behalf of user**   

7. Edit appsettings.json in console project and add in your application ID.

8. Edit ChannelStore.json file to add your private channel information. 





