using GloomyTale.Communication;
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Networking
{
    public class CommunicationServiceEvents
    {
        private static CommunicationServiceEvents _instance;
        public static CommunicationServiceEvents Instance => _instance ?? (_instance = new Lazy<CommunicationServiceEvents>(() => new CommunicationServiceEvents()).Value);

        public event EventHandler BazaarRefresh;

        public event EventHandler CharacterConnectedEvent;

        public event EventHandler CharacterDisconnectedEvent;

        public event EventHandler FamilyRefresh;

        public event EventHandler MessageSentToCharacter;

        public event EventHandler MailSent;

        public event EventHandler AuthorityChange;

        public event EventHandler PenaltyLogRefresh;

        public event EventHandler RelationRefresh;

        public event EventHandler SessionKickedEvent;

        public event EventHandler ShutdownEvent;


        public void OnCharacterConnected(long characterId)
        {
            string characterName = DAOFactory.Instance.CharacterDAO.LoadById(characterId)?.Name;
            CharacterConnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        public void OnCharacterDisconnected(long characterId)
        {
            string characterName = DAOFactory.Instance.CharacterDAO.LoadById(characterId)?.Name;
            CharacterDisconnectedEvent?.Invoke(new Tuple<long, string>(characterId, characterName), null);
        }

        public void OnKickSession(long? accountId, long? sessionId)
        {
            SessionKickedEvent?.Invoke(new Tuple<long?, long?>(accountId, sessionId), null);
        }

        public void OnSendMessageToCharacter(SCSCharacterMessage message)
        {
            MessageSentToCharacter?.Invoke(message, null);
        }

        public void OnUpdateBazaar(long bazaarItemId)
        {
            BazaarRefresh?.Invoke(bazaarItemId, null);
        }

        public void OnUpdateFamily(long familyId, bool changeFaction)
        {
            Tuple<long, bool> tu = new Tuple<long, bool>(familyId, changeFaction);
            FamilyRefresh?.Invoke(tu, null);
        }

        public void OnUpdatePenaltyLog(int penaltyLogId)
        {
            PenaltyLogRefresh?.Invoke(penaltyLogId, null);
        }

        public void OnUpdateRelation(long relationId)
        {
            RelationRefresh?.Invoke(relationId, null);
        }

        public void OnSendMail(MailDTO mail)
        {
            MailSent?.Invoke(mail, null);
        }

        public void OnAuthorityChange(long accountId, AuthorityType authority)
        {
            Tuple<long, AuthorityType> tu = new Tuple<long, AuthorityType>(accountId, authority);
            AuthorityChange?.Invoke(tu, null);
        }

        public void OnShutdown()
        {
            ShutdownEvent?.Invoke(null, null);
        }
    }

    public class CommunicationServiceClient
    {
        public static ICommunicationService Instance { get; private set; }

        public static void Initialize(ICommunicationService service)
        {
            Instance = service;
        }
    }
}
