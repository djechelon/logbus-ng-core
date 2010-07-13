xsd /c /namespace:It.Unina.Dis.Logbus.Configuration logbus-configuration.xsd logbus-filter.xsd
del logbus-configuration.xsd.cs
ren logbus-configuration_logbus-filter.cs logbus-configuration.xsd.cs