mergeInto(LibraryManager.library, {
    LoginFromUnity: function(urlPtr, jwtPtr) {
        const url = UTF8ToString(urlPtr);
        const jwt = UTF8ToString(jwtPtr);
        if (typeof MyUnityBridge !== "undefined" && typeof MyUnityBridge.login === "function") {
            MyUnityBridge.login(url, jwt).then(result => {
                SendMessage("MiniAppBridge", "OnLoginResult", JSON.stringify({
                    status: "success",
                    jwt: result.jwt,
                    wallet: result.wallet_address
                }));
            }).catch(error => {
                SendMessage("MiniAppBridge", "OnLoginResult", JSON.stringify({
                    status: "error",
                    message: error.message || String(error)
                }));
            });
        } else {
            SendMessage("MiniAppBridge", "OnLoginResult", JSON.stringify({
                status: "error",
                message: "Function login not found"
            }));
        }
    },

    DepositFromUnity: function (contractPtr, tokenPtr, amountPtr, payloadPtr) {
        const contract = UTF8ToString(contractPtr);
        const token = UTF8ToString(tokenPtr);
        const amount = parseFloat(UTF8ToString(amountPtr));
        const payload = parseInt(UTF8ToString(payloadPtr));

        if (typeof deposit === "function") {
            deposit(contract, token, amount, payload).then(result => {
                const msg = JSON.stringify({
                    status: "success",
                    hash: result.hash
                });
                SendMessage("MiniAppBridge", "OnDepositResult", msg);
            }).catch(error => {
                const msg = JSON.stringify({
                    status: "error",
                    message: error.message || String(error)
                });
                SendMessage("MiniAppBridge", "OnDepositResult", msg);
            });
        } else {
            SendMessage("MiniAppBridge", "OnDepositResult", JSON.stringify({
                status: "error",
                message: "Function deposit not found"
            }));
        }
    },

    GetTokenBalanceFromUnity: function (tokenPtr, walletPtr) {
        const token = UTF8ToString(tokenPtr);
        const wallet = UTF8ToString(walletPtr);

        if (typeof getTokenBalance === "function") {
            getTokenBalance(token, wallet).then(balance => {
                const msg = JSON.stringify({
                    status: "success",
                    balance: balance
                });
                SendMessage("MiniAppBridge", "OnGetTokenBalanceResult", msg);
            }).catch(error => {
                const msg = JSON.stringify({
                    status: "error",
                    message: error.message || String(error)
                });
                SendMessage("MiniAppBridge", "OnGetTokenBalanceResult", msg);
            });
        } else {
            SendMessage("MiniAppBridge", "OnGetTokenBalanceResult", JSON.stringify({
                status: "error",
                message: "Function getTokenBalance not found"
            }));
        }
    },

    GetUserDataFromUnity: function(userAddressPtr, callbackFuncPtr) {
    var userAddress = UTF8ToString(userAddressPtr);
    window.MyUnityBridge.getUserData(userAddress)
      .then(function(result) {
        var resultStr = JSON.stringify(result);
        var strPtr = stringToNewUTF8(resultStr);
        dynCall('vi', callbackFuncPtr, [strPtr]);
      })
      .catch(function (error) {
        var errorStr = JSON.stringify({ error: error.toString() });
        var strPtr = stringToNewUTF8(errorStr);
        dynCall('vi', callbackFuncPtr, [strPtr]);
      });
    },

    OpenSwapWldFromUnity: function() {
        if(window.MyUnityBridge && window.MyUnityBridge.openSwapWld)
            window.MyUnityBridge.openSwapWld();
    },

    ClaimFromUnity: function(contractPtr, payloadPtr) {
        const contract = UTF8ToString(contractPtr);
        const payloadStr = UTF8ToString(payloadPtr);

        let payload;
        try {
            payload = JSON.parse(payloadStr);
        } catch (err) {
            SendMessage("MiniAppBridge", "OnClaimResult", JSON.stringify({
                status: "error",
                message: "Payload JSON parse error"
            }));
            return;
        }

        if (typeof MyUnityBridge !== "undefined" && typeof MyUnityBridge.claim === "function") {
            MyUnityBridge.claim(contract, payload).then(result => {
                SendMessage("MiniAppBridge", "OnClaimResult", JSON.stringify({
                    status: "success",
                    hash: result.hash
                }));
            }).catch(error => {
                SendMessage("MiniAppBridge", "OnClaimResult", JSON.stringify({
                    status: "error",
                    message: error.message || String(error)
                }));
            });
        } else {
            SendMessage("MiniAppBridge", "OnClaimResult", JSON.stringify({
                status: "error",
                message: "Function claim not found"
            }));
        }
    }
});
