﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenNos.Master.Server.Resource {
    using System;
    
    
    /// <summary>
    ///   Classe di risorse fortemente tipizzata per la ricerca di stringhe localizzate e così via.
    /// </summary>
    // Questa classe è stata generata automaticamente dalla classe StronglyTypedResourceBuilder.
    // tramite uno strumento quale ResGen o Visual Studio.
    // Per aggiungere o rimuovere un membro, modificare il file con estensione ResX ed eseguire nuovamente ResGen
    // con l'opzione /str oppure ricompilare il progetto VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class LocalizedResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LocalizedResources() {
        }
        
        /// <summary>
        ///   Restituisce l'istanza di ResourceManager nella cache utilizzata da questa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenNos.Master.Server.Resource.LocalizedResources", typeof(LocalizedResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Esegue l'override della proprietà CurrentUICulture del thread corrente per tutte le
        ///   ricerche di risorse eseguite utilizzando questa classe di risorse fortemente tipizzata.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Account is already connected..
        /// </summary>
        internal static string ALREADY_CONNECTED {
            get {
                return ResourceManager.GetString("ALREADY_CONNECTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a You have been banned. Reason {0}, until {1}.
        /// </summary>
        internal static string BANNED {
            get {
                return ResourceManager.GetString("BANNED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a You can not talk to the opposite faction..
        /// </summary>
        internal static string CANT_TALK_OPPOSITE_FACTION {
            get {
                return ResourceManager.GetString("CANT_TALK_OPPOSITE_FACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Connection closed by Client..
        /// </summary>
        internal static string CLIENT_CLOSED {
            get {
                return ResourceManager.GetString("CLIENT_CLOSED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Connection closed..
        /// </summary>
        internal static string CLIENT_DISCONNECTED {
            get {
                return ResourceManager.GetString("CLIENT_DISCONNECTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Config loaded!.
        /// </summary>
        internal static string CONFIG_LOADED {
            get {
                return ResourceManager.GetString("CONFIG_LOADED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a CONNECT {0} Connected -- SessionId: {1}.
        /// </summary>
        internal static string CONNECTION {
            get {
                return ResourceManager.GetString("CONNECTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Connection closed..
        /// </summary>
        internal static string CONNECTION_CLOSED {
            get {
                return ResourceManager.GetString("CONNECTION_CLOSED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Connection to the server has been lost..
        /// </summary>
        internal static string CONNECTION_LOST {
            get {
                return ResourceManager.GetString("CONNECTION_LOST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Packet with Header {0} is corrupt or PacketDefinition is invalid. Content: {1} .
        /// </summary>
        internal static string CORRUPT_PACKET {
            get {
                return ResourceManager.GetString("CORRUPT_PACKET", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Database has been initialized!.
        /// </summary>
        internal static string DATABASE_INITIALIZED {
            get {
                return ResourceManager.GetString("DATABASE_INITIALIZED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Database may not be up to date. Please consider updating your database..
        /// </summary>
        internal static string DATABASE_NOT_UPTODATE {
            get {
                return ResourceManager.GetString("DATABASE_NOT_UPTODATE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Client disconnected! ClientId = .
        /// </summary>
        internal static string DISCONNECT {
            get {
                return ResourceManager.GetString("DISCONNECT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Forced Disconnecting of client {0}, too much connections..
        /// </summary>
        internal static string FORCED_DISCONNECT {
            get {
                return ResourceManager.GetString("FORCED_DISCONNECT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Wrong Id or Password!.
        /// </summary>
        internal static string IDERROR {
            get {
                return ResourceManager.GetString("IDERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Server is currently under maintenance. Maintenance estimated time {0}.
        /// </summary>
        internal static string MAINTENANCE {
            get {
                return ResourceManager.GetString("MAINTENANCE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Message received {0} on client {1}.
        /// </summary>
        internal static string MESSAGE_RECEIVED {
            get {
                return ResourceManager.GetString("MESSAGE_RECEIVED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Message sent to client .
        /// </summary>
        internal static string MSG_SENT {
            get {
                return ResourceManager.GetString("MSG_SENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a New client connected. ClientId = .
        /// </summary>
        internal static string NEW_CONNECT {
            get {
                return ResourceManager.GetString("NEW_CONNECT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Account not validate!.
        /// </summary>
        internal static string NOTVALIDATE {
            get {
                return ResourceManager.GetString("NOTVALIDATE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Failed to send packet {0} to client {1}, {2}..
        /// </summary>
        internal static string PACKET_FAILURE {
            get {
                return ResourceManager.GetString("PACKET_FAILURE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a MasterServer started successfully..
        /// </summary>
        internal static string STARTED {
            get {
                return ResourceManager.GetString("STARTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Message is too big ({0} bytes). Max allowed length is {1} bytes..
        /// </summary>
        internal static string TOO_BIG {
            get {
                return ResourceManager.GetString("TOO_BIG", resourceCulture);
            }
        }
    }
}
