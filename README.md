# Maximus.Research.ManagedModules

### A project to research SCOM (System Center Operations Manager) Managed Modules life cycle.

[Blog article for this research](https://maxcorehome.wordpress.com/2020/07/23/implementing-scom-managed-modules-part-1/)

Results of this research let me create a base library for implementing managed modules and console extensions. [Base library: Maximus.Base.Library](https://github.com/MaxxVolk/Maximus.Base.Library)

## Introduction

In general, for any type of monitoring, a variety of testing probes required. Such probes can either actively pull some health metrics such as resource usage, or perform tests like pings or synthetic transactions, or wait for particular events. Later result from these probes get analysed, and used to raise or resolve alerts. System Center Operations Manager (SCOM) works similar way. It has few ways to implement testing probes. The easiest way is to use scripts, such as VBScript, JSScript, or PowerShell. This way is chosen by many SCOM developers, as it can deliver quick results. Script probes are also easy to implement, have less influence to the SCOM Agent itself (i.e. cannot crash or freeze it), they are easy to debug and have some other advantages. So, script probes are highly recommended for beginner developers, or for any rarely running and simple probes.

However, in some circumstances, script probes can turn into culprit, and put too much footprint on monitored system. Or a developer may run into a situation, when required APIs are not available from scripting languages. In all these cases, managed module testing probes can offer much better performance and wider capability range. But at the down side, they are harder to implement and debug, and require a developer to be more careful. This is why managed modules are not that wide spread, despite all their advantages.

This experimental project, and the base library, built on it, is aiming to break the wall for all SCOM developers and let them creating managed modules easy.
