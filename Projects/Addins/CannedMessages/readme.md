# Canned Messages
Canned Messages is an ashscan plugin that allows users to save canned messages of their choice so it saves them typing on repetitious messages.


### Usage
Each user maintains his/her own canned messages by using channel commands. Each channel command starts with a period or dot (.).

### Adding a new Canned Message
Users have to come up with a unique handle for a canned message. Handle only have to be unique among users. So one handle that has used by a user can be used by another.
Once a handle is chosen, users can simply use the following command to add a message

.can add &lt;handle&gt; &lt;message&gt;


#### Add Examples
```
.can add aslresponse My A/S/L is 16/M/Paris, and yours?
.can add rules Please be mindful of the rules, the rules are: No swearing, Abusing, Soliciting, Religious discussion, or Harassment.
```


### Using a canned messsage
There are two ways to use a canned message, one is if you want the bot to say it in the channel without addressing anyone. Other one is if you want it to be addressed to a particular user.

To make the bot say the canned message without addressed to anyone

.can &lt;handle&gt;

To make the bot say the canned message addressed to a user

.can &lt;handle&gt; &lt;nick&gt;


#### Usage Examples
```
.can aslresponse
.can rules Diabolic
```

### Modifying an existing canned message
You can modify an existing canned message that you added by using the following command.

.can modify &lt;handle&gt; &lt;new message&gt;

### Deleting an existing canned message
You can delete a canned message using the following command

.can delete &lt;handle&gt;
