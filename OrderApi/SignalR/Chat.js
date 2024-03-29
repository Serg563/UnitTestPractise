import React, { useEffect, useRef, useState } from "react";
import "../../../CSSs/HomePage/Chat.css";
import chat from "./Images/chat.png";
import userImg from "./Images/user.png";
import close from "./Images/x.png";
import search from "./Images/search.png";
import back from "./Images/back-arrow.png";
import sendMessageImg from "./Images/send-message.png";
import addContact from "./Images/add-contact.png";
import exit from "./Images/exit.png";
import addGroupImg from "./Images/add-group.png";
import { userModel } from "../../../Interfaces";
import { useSelector } from "react-redux";
import { RootState } from "../../../Storage/Redux/store";
import { HubConnectionBuilder } from "@microsoft/signalr";

interface Message {
  messageId: number;
  messageText: string;
  sentDateTime: Date;
  groupId?: number | null;
  group?: null;
  userId?: string | null;
  user?: null;
}
export default function Chat() {
  const [openChatForm, setOpenChatForm] = useState<boolean>(true);
  const [openChat, setOpenChat] = useState<boolean>(true);
  const ref = useRef<HTMLDivElement>(null);
  const [searchedName, setSearchedName] = useState("");
  const [findMode, setFindMode] = useState<boolean>(false);
  const [showPopupContacts, setShowPopupContacts] = useState(false);
  const [selectedContact, setSelectedContact] = useState("");
  const [createGroup, setCreateGroup] = useState<string>("");
  const [isOpenAddGroup, setIsOpenAddGroup] = useState<boolean>(false);

  const userData: userModel = useSelector(
    (state: RootState) => state.userAuthStore
  );

  const [chatUsers, setChatUsers] = useState([
    {
      id: 1,
      image: userImg,
      name: "John",
      message: "Hello!",
      lastMessageTime: "10:30 AM",
      newMessageCount: 2,
    },
    {
      id: 2,
      image: userImg,
      name: "Alice",
      message: "Hey there!",
      lastMessageTime: "Yesterday",
      newMessageCount: 0,
    },
    // Add more chat users...
    {
      id: 3,
      image: userImg,
      name: "Bob",
      message: "How are you?",
      lastMessageTime: "12:00 PM",
      newMessageCount: 1,
    },
    {
      id: 4,
      image: userImg,
      name: "Emily",
      message: "Hi!",
      lastMessageTime: "2:15 PM",
      newMessageCount: 0,
    },
    // Add more chat users...
    {
      id: 5,
      image: userImg,
      name: "Sarah",
      message: "What's up?",
      lastMessageTime: "3:30 PM",
      newMessageCount: 3,
    },
    {
      id: 6,
      image: userImg,
      name: "Michael",
      message: "Good morning!",
      lastMessageTime: "9:00 AM",
      newMessageCount: 0,
    },
    // Add more chat users...
    {
      id: 7,
      image: userImg,
      name: "Emma",
      message: "Nice to meet you!",
      lastMessageTime: "Yesterday",
      newMessageCount: 1,
    },
    {
      id: 8,
      image: userImg,
      name: "David",
      message: "Howdy!",
      lastMessageTime: "4:45 PM",
      newMessageCount: 0,
    },
    // Add more chat users...
    {
      id: 9,
      image: userImg,
      name: "Olivia",
      message: "Good evening!",
      lastMessageTime: "Yesterday",
      newMessageCount: 2,
    },
    {
      id: 10,
      image: userImg,
      name: "Daniel",
      message: "Hi there!",
      lastMessageTime: "11:30 AM",
      newMessageCount: 0,
    },
  ]);

  useEffect(() => {
    const test = (event: Event) => {
      if (ref.current && !ref.current.contains(event.target as any)) {
        setOpenChatForm(!openChatForm);
      }
    };
    document.addEventListener("click", test, true);
    return () => {
      document.removeEventListener("click", test, true);
      setOpenChat(true);
    };
  }, [openChatForm]);

  // ---------------------------------------FETCH CALLS--------------------------------------

  const [users, setUsers] = useState([]);
  const [messagesBack, setMessagesBack] = useState([]);
  const [everything, setEverything] = useState([]);
  const [sendMessage, setSendMessage] = useState("");
  const [groupIdState, setGroupIdState] = useState<number>();
  const [groups, setGroups] = useState([]);

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await fetch(
          "http://localhost:5019/GetAllUsersWithDetail"
        );
        const data = await response.json();
        setUsers(data);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    }

    fetchData();
  }, []);

  useEffect(() => {
    async function fetchMessages() {
      try {
        const response = await fetch(
          `http://localhost:5019/GetUsersChatsWithMembersAndMessages?userId=${userData.id}`
        );
        if (!response.ok) {
          throw new Error("Failed to fetch messages");
        }
        const data = await response.json();
        setEverything(data);
        console.log("messages back: ", messagesBack);
      } catch (error) {
        console.error("Error fetching messages:", error);
      }
    }

    fetchMessages();
  }, []);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch("http://localhost:5019/GetAllGroups");
        if (!response.ok) {
          throw new Error("Failed to fetch groups");
        }
        const data = await response.json();
        setGroups(data); // Assuming data is an array of groups
      } catch (error) {
        console.error("Error fetching groups:", error);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    console.log("everything", everything);
  }, [everything]);

  const OnLiClick = async (groupId: number) => {
    setGroupIdState(groupId);
    getChatMessages(groupId);
    setOpenChat(false);
  };

  // ------------------------------------------SIGNALR---------------------------------------------------

  const [connection, setConnection] = useState<any>(null);
  const [signalRMessages, setSignalRMessages] = useState<Message[]>([]);

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl("http://localhost:5019/chat")
      .build();

    conn.on("ReceiveMessage", (m: Message) => {
      setSignalRMessages((p) => [...p, m]);
      console.log("Updated messages:", signalRMessages);
    });

    conn
      .start()
      .then(() => {
        setConnection(conn);
      })
      .catch((error) => {
        console.error("Connection error:", error);
      });

    // Clean up on unmount
    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, []);

  const getChatMessages = (groupId: number) => {
    setSignalRMessages([]);
    connection && connection.send("GetChatMessages", groupId);
    console.log("GetChatMessages", signalRMessages);
  };

  const signalrSendMessage = (groupId: number, msg: string) => {
    setSendMessage("");
    connection && connection.send("SendMessageToGroup", groupId, msg);
  };
  const signalrCreateGroup = (groupName: string) => {
    connection && connection.send("CreateGroup", groupName);
  };
  const signalrLeaveGroup = (groupId: number, userId: string) => {
    connection && connection.send("LeaveGroup", groupId, userId);
    setOpenChat(true);
  };

  return (
    <>
      {openChatForm ? (
        <img
          className="chat-img"
          src={chat}
          onClick={() => setOpenChatForm(!openChatForm)}
        ></img>
      ) : (
        <div className="chat-container" ref={ref}>
          {openChat ? (
            <>
              {findMode ? (
                <div className="find-container">
                  <input
                    placeholder="Find Contact..."
                    // onFocus={() => setShowPopupContacts(true)}
                    // onBlur={() => setShowPopupContacts(false)}
                    onChange={(e) => {
                      setSelectedContact(e.target.value);
                      setShowPopupContacts(true);
                    }}
                  ></input>
                  <img
                    src={close}
                    onClick={() => {
                      setShowPopupContacts(false);
                      setFindMode(false);
                    }}
                  ></img>
                  {showPopupContacts && (
                    <div className="popup-contacts-menu">
                      {users
                        .filter((user: any) =>
                          user.name
                            .toLowerCase()
                            .includes(selectedContact.toLowerCase())
                        )
                        .map((user: any) => (
                          <div
                            // key={user.id}
                            className="popup-item"
                            onClick={() => {
                              setChatUsers((prev) => [user, ...prev]);
                              setShowPopupContacts(false);
                            }}
                          >
                            <div style={{ lineHeight: "70px" }}>
                              <img src={userImg} />
                            </div>
                            <div className="info-container">
                              <p className="user-name">{user.name}</p>
                              <p style={{ color: "green" }}>Online</p>
                            </div>
                          </div>
                        ))}
                      {groups
                        .filter((group: any) =>
                          group.groupName
                            ? group.groupName
                                .toLowerCase()
                                .includes(selectedContact.toLowerCase())
                            : false
                        )
                        .map((group: any) => (
                          <div
                            className="popup-item"
                            onClick={() => {
                              // Handle group selection
                              setShowPopupContacts(false);
                            }}
                          >
                            <div style={{ lineHeight: "70px" }}>
                              <img src={userImg} />
                            </div>
                            <div className="info-container">
                              <p className="group-name">{group.groupName}</p>
                            </div>
                          </div>
                        ))}
                    </div>
                  )}
                </div>
              ) : (
                <div className="search-container">
                  <input
                    type="text"
                    placeholder="Search"
                    onChange={(e: any) => setSearchedName(e.target.value)}
                  />
                  <img src={search} className="search-img" />

                  <img
                    src={addContact}
                    className="add-contact-img"
                    onClick={() => setFindMode(true)}
                  ></img>
                </div>
              )}

              <ul className={showPopupContacts ? "blur-ul" : ""}>
                {everything
                  // .filter((f: any) =>
                  //   f.groupMembers[0]
                  //     .user!.email!.toLowerCase()
                  //     .startsWith(searchedName.toLowerCase())
                  // )
                  .filter((f: any) =>
                    f.groupName
                      ? f.groupName
                          .toLowerCase()
                          .startsWith(searchedName.toLowerCase())
                      : f.groupMembers[0]
                          .user!.email!.toLowerCase()
                          .startsWith(searchedName.toLowerCase())
                  )
                  .map((user: any, key: any) => (
                    <li key={key} onClick={() => OnLiClick(user.groupId)}>
                      <div style={{ lineHeight: "70px" }}>
                        {/* <img src={user.image} className="user-img" /> */}
                        <img src={userImg} className="user-img" />
                      </div>
                      <div className="chat-preview">
                        <div className="header">
                          {/* <p>{user.name}</p> */}
                          {/* <p>{user.lastMessageTime}</p> */}
                          <p>
                            {user.groupName
                              ? user.groupName
                              : user.groupMembers[0].user!.email!}
                          </p>
                          {user.messages &&
                            user.messages.length > 0 &&
                            user.messages[user.messages.length - 1]
                              .sentDateTime && (
                              <p>
                                {new Date(
                                  user.messages[
                                    user.messages.length - 1
                                  ].sentDateTime
                                ).toLocaleTimeString()}
                              </p>
                            )}
                        </div>
                        <div className="body">
                          <p>
                            {user.messages[user.messages.length - 1]
                              ?.messageText.length > 30
                              ? `${user.messages[
                                  user.messages.length - 1
                                ].messageText.substring(0, 30)}...`
                              : user.messages[user.messages.length - 1]
                                  ?.messageText}
                          </p>
                          <p>{user.messages.length}</p>
                        </div>
                      </div>
                    </li>
                  ))}
                {!isOpenAddGroup ? (
                  <img
                    src={addGroupImg}
                    title="Create Group"
                    onClick={() => signalrCreateGroup("test")}
                  ></img>
                ) : (
                  <input
                    style={{
                      position: "absolute",
                      bottom: "5px",
                      right: "10px",
                    }}
                  ></input>
                )}
              </ul>
            </>
          ) : (
            <div>
              <div className="chat-header">
                <img src={back} onClick={() => setOpenChat(true)}></img>
                <div style={{ margin: "auto" }}>User Name</div>
                <img
                  src={exit}
                  style={{ filter: "contrast(200%)" }}
                  onClick={() => signalrLeaveGroup(groupIdState!, userData.id)}
                ></img>
              </div>
              {/* <div className="chat-content">{renderMessages()}</div> */}
              <div className="chat-content">
                {signalRMessages &&
                  signalRMessages.map((message: Message, key: number) => {
                    // console.log("Message:", message);
                    return (
                      <div
                        key={message.messageId}
                        className={
                          message.userId === userData.id
                            ? "message message-right"
                            : "message message-left"
                        }
                      >
                        <p
                          style={{ paddingBottom: "0px", marginBottom: "0px" }}
                        >
                          {message.messageText}
                        </p>
                        <p style={{ margin: "0px", marginLeft: "auto" }}>
                          {new Date(message.sentDateTime).toLocaleTimeString()}
                        </p>
                      </div>
                    );
                  })}
              </div>
              <div className="chat-footer">
                <input
                  placeholder="Send message..."
                  onChange={(e: any) => setSendMessage(e.target.value)}
                  value={sendMessage}
                ></input>
                <img
                  src={sendMessageImg}
                  onClick={() => signalrSendMessage(groupIdState!, sendMessage)}
                ></img>
                {sendMessage}
              </div>
            </div>
          )}
        </div>
      )}

      {/* <div className="chat-container">Chat</div> */}
    </>
  );
}
