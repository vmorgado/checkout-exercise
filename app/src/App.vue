<template>
  <v-app>
    <v-main>
      <PaymentForm/>
    </v-main>
  </v-app>
</template>

<script>
import PaymentForm from './components/PaymentForm';

export default {
  name: 'App',

  components: {
    PaymentForm,
  },

  data: () => ({
    //
  }),
};
</script>
