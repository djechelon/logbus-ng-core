wsdl /serverInterface /namespace:It.Unina.Dis.Logbus.WebServices logbus-control.wsdl logbus-filter.xsd
del logbus-control.cs
ren Interfaces.cs logbus-control.cs