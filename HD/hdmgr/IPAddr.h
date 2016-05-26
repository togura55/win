#pragma once


// IPAddr

class IPAddress : public CIPAddressCtrl
{
	DECLARE_DYNAMIC(IPAddress)

public:
	IPAddress();
	virtual ~IPAddress();

protected:
	DECLARE_MESSAGE_MAP()
};


