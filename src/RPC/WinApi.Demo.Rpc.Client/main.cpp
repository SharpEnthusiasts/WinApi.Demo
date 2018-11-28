#include <iostream>
#include "example_c.c"

int main()
{
	RPC_STATUS status;
	unsigned char* szStringBinding = NULL;

	// Creates a string binding handle.
	// This function is nothing more than a printf.
	// Connection is not done here.
	status = RpcStringBindingCompose(
		NULL, // UUID to bind to.
		(RPC_WSTR)L"ncacn_ip_tcp", // Use TCP/IP protocol.
		(RPC_WSTR)L"localhost", // TCP/IP network address to use.
		(RPC_WSTR)L"4747", // TCP/IP port to use.
		NULL, // Protocol dependent network options to use.
		(RPC_WSTR*)&szStringBinding); // String binding output.

	if (status)
		exit(status);

	// Validates the format of the string binding handle and converts
	// it to a binding handle.
	// Connection is not done here either.
	status = RpcBindingFromStringBinding(
		(RPC_WSTR)szStringBinding, // The string binding to validate.
		&hExampleBinding); // Put the result in the implicit binding
							// handle defined in the IDL file.

	if (status)
		exit(status);

	bool stop = false;
	while (true)
	{
		char c = std::cin.get();
		switch (c)
		{
		case '0':
			stop = true;
			break;

		default:
			break;
		}

		if (stop)
		{
			break;
		}

		RpcTryExcept
		{
			std::cout << "Calling server" << std::endl;

		// Calls the RPC function. The hExample1Binding binding handle
		// is used implicitly.
		// Connection is done here.
		Output(reinterpret_cast<unsigned const char*>("Sever called"));
		}
			RpcExcept(1)
		{
			std::cerr << "Runtime reported exception " << RpcExceptionCode()
				<< std::endl;
		}
		RpcEndExcept
	}

	// Free the memory allocated by a string.
	status = RpcStringFree(
		(RPC_WSTR*)&szStringBinding); // String to be freed.

	if (status)
		exit(status);

	// Releases binding handle resources and disconnects from the server.
	status = RpcBindingFree(
		&hExampleBinding); // Frees the implicit binding handle defined in the IDL file.

	if (status)
		exit(status);
}

// Memory allocation function for RPC.
// The runtime uses these two functions for allocating/deallocating
// enough memory to pass the string to the server.
void* __RPC_USER midl_user_allocate(size_t size)
{
	return malloc(size);
}

// Memory deallocation function for RPC.
void __RPC_USER midl_user_free(void* p)
{
	free(p);
}