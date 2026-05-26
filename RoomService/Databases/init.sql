--
-- PostgreSQL database dump
--

\restrict 4Q4gFwWsJ5OdI6z3TiHQYAMmYKcTyNvyxkBvamRJTLMidS5Q30V74k3Cg06eoA4

-- Dumped from database version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.14 (Ubuntu 16.14-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: PersonRoom; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."PersonRoom" (
    "PersonsId" text NOT NULL,
    "RoomsId" character varying(5) NOT NULL
);


ALTER TABLE public."PersonRoom" OWNER TO pauldor;

--
-- Name: Persons; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."Persons" (
    "Id" text NOT NULL,
    "Name" text NOT NULL
);


ALTER TABLE public."Persons" OWNER TO pauldor;

--
-- Name: Rooms; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."Rooms" (
    "Id" character varying(5) NOT NULL,
    "AdminId" text NOT NULL,
    "Password" text NOT NULL,
    "MusicId" uuid NOT NULL,
    "IsActive" boolean NOT NULL,
    "ActionTime" timestamp with time zone NOT NULL,
    "Position" integer NOT NULL
);


ALTER TABLE public."Rooms" OWNER TO pauldor;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO pauldor;

--
-- Data for Name: PersonRoom; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."PersonRoom" ("PersonsId", "RoomsId") FROM stdin;
\.


--
-- Data for Name: Persons; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."Persons" ("Id", "Name") FROM stdin;
\.


--
-- Data for Name: Rooms; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."Rooms" ("Id", "AdminId", "Password", "MusicId", "IsActive", "ActionTime", "Position") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260523151755_InitialCreate	10.0.8
20260524105008_changeRoomModel	10.0.8
\.


--
-- Name: PersonRoom PK_PersonRoom; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."PersonRoom"
    ADD CONSTRAINT "PK_PersonRoom" PRIMARY KEY ("PersonsId", "RoomsId");


--
-- Name: Persons PK_Persons; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Persons"
    ADD CONSTRAINT "PK_Persons" PRIMARY KEY ("Id");


--
-- Name: Rooms PK_Rooms; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Rooms"
    ADD CONSTRAINT "PK_Rooms" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_PersonRoom_RoomsId; Type: INDEX; Schema: public; Owner: pauldor
--

CREATE INDEX "IX_PersonRoom_RoomsId" ON public."PersonRoom" USING btree ("RoomsId");


--
-- Name: PersonRoom FK_PersonRoom_Persons_PersonsId; Type: FK CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."PersonRoom"
    ADD CONSTRAINT "FK_PersonRoom_Persons_PersonsId" FOREIGN KEY ("PersonsId") REFERENCES public."Persons"("Id") ON DELETE CASCADE;


--
-- Name: PersonRoom FK_PersonRoom_Rooms_RoomsId; Type: FK CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."PersonRoom"
    ADD CONSTRAINT "FK_PersonRoom_Rooms_RoomsId" FOREIGN KEY ("RoomsId") REFERENCES public."Rooms"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict 4Q4gFwWsJ5OdI6z3TiHQYAMmYKcTyNvyxkBvamRJTLMidS5Q30V74k3Cg06eoA4

