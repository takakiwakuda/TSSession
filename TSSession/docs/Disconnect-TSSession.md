---
external help file: TSSession.dll-Help.xml
Module Name: TSSession
online version: https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Disconnect-TSSession.md
schema: 2.0.0
---

# Disconnect-TSSession

## SYNOPSIS

Disconnects a Remote Desktop Services session.

## SYNTAX

### SessionId (Default)

```powershell
Disconnect-TSSession [-SessionId] <Int32> [[-ServerName] <String>] [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### Session

```powershell
Disconnect-TSSession -Session <TerminalServicesSession> [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION

The `Disconnect-TSSession` cmdlet disconnects a Remote Desktop Services session.

## EXAMPLES

### Example 1

```powershell
PS C:\> Disconnect-TSSession -SessionId 5 -ServerName RDSessionHost
```

This example disconnects a session with id 5 on the server `RDSessionHost`.

## PARAMETERS

### -Force

Does not prompt you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ServerName

Specifies the name of the server hosting session.

```yaml
Type: String
Parameter Sets: SessionId
Aliases: ComputerName, SessionHostName

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Session

Specifies the session.

```yaml
Type: TerminalServicesSession
Parameter Sets: Session
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SessionId

Specifies the session identifier.

```yaml
Type: Int32
Parameter Sets: SessionId
Aliases: Id

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf

Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### TSSession.TerminalServicesSession

## OUTPUTS

### None

## NOTES

## RELATED LINKS

[Get-TSSession](https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Get-TSSession.md)

[Remove-TSSession](https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Remove-TSSession.md)
