window.MyUnityBridge = {
  deposit,
  login,
  getTokenBalance,
  getUserData,
  openSwapWld,
  claim
};

/**
 * Tải toàn bộ folder build từ link sau: https://www.jsdelivr.com/package/npm/@worldcoin/minikit-js?tab=files&path=build
 * const MiniKit = require(dẫn link tời file index.js trong folder build);
 * White list contract address và token Address trong develop portal: token thì trong phần permit2
 * call hàm này khi muốn deposit người dùng call thành công thì sử dụng api của backend để check
 *
 */
async function deposit(contractAddress, tokenAddress, amount, payload, decimals = 18) {
    if (Number(amount) <= 0) {
        throw new Error("Amount must be greater than 0");
    }
    const permitTransfer = {
        permitted: {
            token: tokenAddress,
            amount: ethers.parseUnits(`${amount}`, decimals).toString(),
        },
        nonce: Date.now().toString(),
        deadline: Math.floor((Date.now() + 30 * 60 * 1000) / 1000).toString(),
    };
    const { finalPayload } = await MiniKit.commandsAsync.sendTransaction({
        transaction: [
            {
                address: contractAddress,
                abi: [
                    {
                        type: "function",
                        name: "deposit",
                        inputs: [
                            {
                                name: "token",
                                type: "address",
                                internalType: "address",
                            },
                            {
                                name: "amount",
                                type: "uint256",
                                internalType: "uint256",
                            },
                            {
                                name: "payload",
                                type: "uint256",
                                internalType: "uint256",
                            },
                            {
                                name: "permit2Signature",
                                type: "bytes",
                                internalType: "bytes",
                            },
                            {
                                name: "permit",
                                type: "tuple",
                                internalType: "struct ISignatureTransfer.PermitTransferFrom",
                                components: [
                                    {
                                        name: "permitted",
                                        type: "tuple",
                                        internalType: "struct ISignatureTransfer.TokenPermissions",
                                        components: [
                                            {
                                                name: "token",
                                                type: "address",
                                                internalType: "address",
                                            },
                                            {
                                                name: "amount",
                                                type: "uint256",
                                                internalType: "uint256",
                                            },
                                        ],
                                    },
                                    {
                                        name: "nonce",
                                        type: "uint256",
                                        internalType: "uint256",
                                    },
                                    {
                                        name: "deadline",
                                        type: "uint256",
                                        internalType: "uint256",
                                    },
                                ],
                            },
                        ],
                        outputs: [],
                        stateMutability: "payable",
                    },
                ],
                functionName: "deposit",
                args: [
                    tokenAddress,
                    permitTransfer.permitted.amount,
                    payload.toString(),
                    "PERMIT2_SIGNATURE_PLACEHOLDER_0",
                    [
                        [permitTransfer.permitted.token, permitTransfer.permitted.amount],
                        permitTransfer.nonce,
                        permitTransfer.deadline,
                    ],
                ],
            },
        ],
        permit2: [
            {
                ...permitTransfer,
                spender: contractAddress,
            },
        ],
        formatPayload: false,
    });
    if (finalPayload.status === "error") {
        throw new Error(finalPayload.error_code);
    }
    return {
        hash: finalPayload.transaction_id,
    };
}

async function login(baseUrl, jwt) {
    const walletAuthInput = (nonce) => {
        return {
            nonce,
            requestId: "0",
            expirationTime: new Date(new Date().getTime() + 7 * 24 * 60 * 60 * 1000),
            notBefore: new Date(new Date().getTime() - 24 * 60 * 60 * 1000),
            statement: "This is my statement and here is a link https://worldcoin.com/apps",
        };
    };
    try {
        const nonceRes = await fetch(`${baseUrl}/api/auth/nonce`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        });
        if (!nonceRes.ok) {
            throw new Error("Failed to fetch nonce");
        }
        const { nonce } = await nonceRes.json();
        if (!nonce) {
            throw new Error("Nonce is required");
        }
        const { commandPayload: _, finalPayload } = await MiniKit.commandsAsync.walletAuth(walletAuthInput(nonce));
        if (finalPayload.status === "error") {
            throw new Error(JSON.stringify(finalPayload.details));
        }

        const res = await fetch(`${baseUrl}/api/auth`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                nonce: nonce,
                payload: finalPayload,
            }),
        });

        if (!res.ok) {
            throw new Error("Failed to login");
        }
        const { jwt } = await res.json();

        if (!jwt) {
            throw new Error("JWT token is required");
        }

        return {
            jwt: jwt,
            wallet_address: MiniKit.user.walletAddress
        };
    } catch (error) {
        throw error;
    }
}

async function getTokenBalance(tokenAddress, walletAddress) {
    try {
        if (!walletAddress) {
            throw new Error('Wallet address not found!');
        }
        const rpcUrl = 'https://worldchain-mainnet.g.alchemy.com/public'
        const provider = new ethers.JsonRpcProvider(rpcUrl);
        
        // ERC20 ABI for balanceOf function
        const erc20Abi = [
            "function balanceOf(address owner) view returns (uint256)"
        ];
        
        const contract = new ethers.Contract(tokenAddress, erc20Abi, provider);
        const balance = await contract.balanceOf(walletAddress);
        
        // Return balance as string to preserve precision
        return ethers.formatEther(balance);
    } catch (error) {
        console.error('Error getting token balance:', error);
        throw error;
    }
}


async function getUserData(userAddress) {
    try {
        console.log("[getUserData] input:", userAddress);
        const data = await MiniKit.getUserByAddress(userAddress);
        //console.log("[getUserData] result:", data);
        return {
            username: data.username,
            avatar: data.profilePictureUrl
        }
    } catch (error) {
        console.error('Error getting world app user data:', error);
        throw error;
    }
}

function openSwapWld() {
    const url = "https://world.org/mini-app?app_id=app_0d4b759921490adc1f2bd569fda9b53a&path=%2Ftoken%2Fbuy%3Faddress%3D0x2cFc85d8E48F8EAB294be644d9E25C3030863003";
    const newWindow = window.open(url, "_blank");
    if (newWindow) {
        newWindow.focus();
    } else {
        console.error("Failed to open swap link in a new tab.");
    }
}

async function claim(contractAddress, payload) {
    const { finalPayload } = await MiniKit.commandsAsync.sendTransaction({
        transaction: [
            {
                address: contractAddress,
                abi: [
                    {
                        "type": "function",
                        "name": "claim",
                        "inputs": [
                          {
                            "name": "adminAddress",
                            "type": "address",
                            "internalType": "address"
                          },
                          {
                            "name": "token",
                            "type": "address",
                            "internalType": "address"
                          },
                          {
                            "name": "id",
                            "type": "uint256",
                            "internalType": "uint256"
                          },
                          {
                            "name": "chainId_",
                            "type": "uint256",
                            "internalType": "uint256"
                          },
                          {
                            "name": "claimAmount",
                            "type": "uint256",
                            "internalType": "uint256"
                          },
                          {
                            "name": "payload",
                            "type": "uint256",
                            "internalType": "uint256"
                          },
                          {
                            "name": "expiredBlockNumber",
                            "type": "uint256",
                            "internalType": "uint256"
                          },
                          {
                            "name": "recipient",
                            "type": "address",
                            "internalType": "address"
                          },
                          {
                            "name": "signature",
                            "type": "bytes",
                            "internalType": "bytes"
                          }
                        ],
                        "outputs": [],
                        "stateMutability": "nonpayable"
                      }
                ],
                functionName: "claim",
                args: [
                    payload.adminAddress,
                    payload.tokenAddress,
                    payload.id,
                    payload.chainId,
                    payload.amount,
                    payload.payload,
                    payload.expiredBlockNumber,
                    payload.recipient,
                    payload.signature
                ],
            },
        ],
        formatPayload: false,
    });

    if (finalPayload.status === "error") {
        throw new Error(JSON.stringify(finalPayload));
    }

    return {
        hash: finalPayload.transaction_id,
    };
}

