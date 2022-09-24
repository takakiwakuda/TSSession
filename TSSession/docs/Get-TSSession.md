---
external help file: TSSession.dll-Help.xml
Module Name: TSSession
online version: https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Get-TSSession.md
schema: 2.0.0
---

# Get-TSSession

## SYNOPSIS

Gets a list of all sessions on a Remote Desktop Session Host server.

## SYNTAX

```powershell
Get-TSSession [[-ServerName] <String>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-TSSession` cmdlet gets a list of all sessions on a Remote Desktop Session Host server.

## EXAMPLES

### Example 1

```powershell
PS C:\> Get-TSSession
```

This example gets a list of all sessions on the local machine.

## PARAMETERS

### -ServerName

Specifies the name of the server hosting session.

```yaml
Type: String
Parameter Sets: (All)
Aliases: ComputerName, SessionHostName

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### TSSession.TerminalServicesSession

## NOTES

## RELATED LINKS

[Disconnect-TSSession](https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Disconnect-TSSession.md)

[Remove-TSSession](https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Remove-TSSession.md)
