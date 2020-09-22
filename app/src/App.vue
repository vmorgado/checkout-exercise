<template>
  <v-app>
    <v-main>
      <BuyProductPage/>
    </v-main>
  </v-app>
</template>

<script>
import BuyProductPage from './components/BuyProductPage';

export default {
  name: 'App',
  components: {
    BuyProductPage
  },
  data: () => ({
    //
  }),
};
</script>
